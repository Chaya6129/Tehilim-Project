import type { AuthResult } from '../api/auth';
import { parseUtcDate } from '../utils/date';
import { getUserIdFromToken } from './jwt';

export interface Session {
  token: string;
  expiresAtUtc: string;
  fullName: string;
  role: string;
  userId: number | null;
}

export type LogoutReason = 'manual' | 'expired' | 'unauthorized';

const STORAGE_KEY = 'tehilim.session';
// setTimeout מוגבל ל-2^31-1 מילישניות. מעבר לכך מתזמנים שוב.
const MAX_TIMEOUT_MS = 2_147_483_647;

type Listener = () => void;
type LogoutListener = (reason: LogoutReason) => void;

let current: Session | null = null;
let expiryTimer: number | undefined;
const listeners = new Set<Listener>();
const logoutListeners = new Set<LogoutListener>();

function isSession(value: unknown): value is Session {
  if (typeof value !== 'object' || value === null) return false;
  const v = value as Record<string, unknown>;
  return (
    typeof v.token === 'string' &&
    typeof v.expiresAtUtc === 'string' &&
    typeof v.fullName === 'string' &&
    typeof v.role === 'string' &&
    (v.userId === null || typeof v.userId === 'number')
  );
}

function isExpired(session: Session): boolean {
  const msLeft = parseUtcDate(session.expiresAtUtc).getTime() - Date.now();
  return !(msLeft > 0);
}

function readStored(): Session | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    const parsed: unknown = JSON.parse(raw);
    return isSession(parsed) ? parsed : null;
  } catch {
    return null;
  }
}

function writeStored(session: Session | null): void {
  try {
    if (session) localStorage.setItem(STORAGE_KEY, JSON.stringify(session));
    else localStorage.removeItem(STORAGE_KEY);
  } catch {
    // localStorage חסום (למשל גלישה פרטית קפדנית). ההתחברות תישמר בזיכרון בלבד.
  }
}

function scheduleExpiry(): void {
  window.clearTimeout(expiryTimer);
  expiryTimer = undefined;
  if (!current) return;
  const msLeft = parseUtcDate(current.expiresAtUtc).getTime() - Date.now();
  if (!(msLeft > 0)) return;
  expiryTimer = window.setTimeout(() => {
    if (current && isExpired(current)) clear('expired');
    else scheduleExpiry();
  }, Math.min(msLeft, MAX_TIMEOUT_MS));
}

function apply(next: Session | null): void {
  current = next;
  scheduleExpiry();
  listeners.forEach((listener) => listener());
}

function clear(reason: LogoutReason): void {
  if (!current) return;
  writeStored(null);
  apply(null);
  logoutListeners.forEach((listener) => listener(reason));
}

export function sessionFromAuthResult(result: AuthResult): Session {
  return {
    token: result.token,
    expiresAtUtc: result.expiresAtUtc,
    fullName: result.fullName ?? '',
    role: result.role ?? 'User',
    userId: getUserIdFromToken(result.token),
  };
}

/**
 * מקור האמת להתחברות, מחוץ ל-React: apiClient קורא ממנו את ה-token,
 * ו-AuthProvider נרשם אליו דרך useSyncExternalStore.
 */
export const sessionStore = {
  subscribe(listener: Listener): () => void {
    listeners.add(listener);
    return () => {
      listeners.delete(listener);
    };
  },

  onLogout(listener: LogoutListener): () => void {
    logoutListeners.add(listener);
    return () => {
      logoutListeners.delete(listener);
    };
  },

  getSnapshot(): Session | null {
    return current;
  },

  set(session: Session): void {
    writeStored(session);
    apply(session);
  },

  clear,

  /** ה-token, רק אם עדיין בתוקף. token שפג גורם להתנתקות. */
  getValidToken(): string | null {
    if (!current) return null;
    if (isExpired(current)) {
      clear('expired');
      return null;
    }
    return current.token;
  },
};

function initialize(): void {
  const stored = readStored();
  if (stored && !isExpired(stored)) {
    current = stored;
    scheduleExpiry();
  } else if (stored) {
    writeStored(null);
  }

  // סנכרון בין לשוניות: התחברות או התנתקות בלשונית אחת משתקפות בכולן.
  window.addEventListener('storage', (event) => {
    if (event.key !== STORAGE_KEY && event.key !== null) return;
    const next = readStored();
    apply(next && !isExpired(next) ? next : null);
  });
}

initialize();
