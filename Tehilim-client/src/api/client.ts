import { sessionStore } from '../auth/sessionStore';

const API_URL = (import.meta.env.VITE_API_URL ?? '').replace(/\/+$/, '');

export type FieldErrors = Record<string, string[]>;

/** סטטוס פנימי לשגיאת רשת: הבקשה לא הגיעה לשרת כלל. */
export const NETWORK_ERROR_STATUS = 0;

export class ApiError extends Error {
  readonly status: number;
  readonly correlationId: string | null;
  readonly errors: FieldErrors | null;

  constructor(init: {
    status: number;
    message: string;
    correlationId?: string | null;
    errors?: FieldErrors | null;
  }) {
    super(init.message);
    this.name = 'ApiError';
    this.status = init.status;
    this.correlationId = init.correlationId ?? null;
    this.errors = init.errors ?? null;
  }
}

const DEFAULT_MESSAGES: Record<number, string> = {
  [NETWORK_ERROR_STATUS]:
    'לא ניתן להתחבר לשרת. ודאו שהשרת פועל ושתעודת ה-HTTPS שלו אושרה בדפדפן.',
  400: 'הנתונים שנשלחו אינם תקינים.',
  401: 'יש להתחבר כדי לבצע פעולה זו.',
  403: 'אין לך הרשאה לבצע פעולה זו.',
  404: 'הפריט המבוקש לא נמצא.',
  409: 'הפעולה אינה אפשרית במצב הנוכחי.',
};

function fallbackMessage(status: number): string {
  return (
    DEFAULT_MESSAGES[status] ??
    (status >= 500 ? 'אירעה תקלה בשרת. נסו שוב בעוד רגע.' : 'אירעה שגיאה לא צפויה.')
  );
}

type QueryValue = string | number | boolean | null | undefined;

export interface RequestOptions {
  method?: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';
  body?: unknown;
  query?: Record<string, QueryValue>;
  signal?: AbortSignal;
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null && !Array.isArray(value);
}

function buildUrl(path: string, query: Record<string, QueryValue> | undefined): string {
  const params = new URLSearchParams();
  for (const [key, value] of Object.entries(query ?? {})) {
    if (value !== undefined && value !== null && value !== '') {
      params.append(key, String(value));
    }
  }
  const queryString = params.toString();
  return `${API_URL}${path}${queryString ? `?${queryString}` : ''}`;
}

async function readJson(response: Response): Promise<unknown> {
  const text = await response.text();
  if (!text) return undefined;
  try {
    return JSON.parse(text) as unknown;
  } catch {
    return undefined;
  }
}

/** "Goal", "$.goal" ו-"CategoryIds[0]" הופכים ל-"goal" ו-"categoryIds", כדי להתאים לשמות השדות בטפסים. */
function normalizeFieldName(key: string): string {
  const cleaned = key.replace(/^\$\.?/, '');
  const head = cleaned.split(/[.[]/)[0] ?? cleaned;
  return head.charAt(0).toLowerCase() + head.slice(1);
}

function parseFieldErrors(value: unknown): FieldErrors | null {
  if (!isRecord(value)) return null;
  const result: FieldErrors = {};
  for (const [key, messages] of Object.entries(value)) {
    const list = Array.isArray(messages)
      ? messages.filter((message): message is string => typeof message === 'string')
      : typeof messages === 'string'
        ? [messages]
        : [];
    if (list.length === 0) continue;
    const name = normalizeFieldName(key);
    result[name] = [...(result[name] ?? []), ...list];
  }
  return Object.keys(result).length > 0 ? result : null;
}

function toApiError(response: Response, body: unknown): ApiError {
  const { status } = response;
  const headerCorrelationId = response.headers.get('X-Correlation-Id');

  // 401 ו-403 של מנגנון ההרשאות מגיעים בלי גוף: מטפלים בהם לפי הסטטוס בלבד.
  if (!isRecord(body)) {
    return new ApiError({ status, message: fallbackMessage(status), correlationId: headerCorrelationId });
  }

  const message = [body.message, body.detail, body.title].find(
    (candidate): candidate is string => typeof candidate === 'string' && candidate.trim() !== '',
  );

  return new ApiError({
    status,
    message: message ?? fallbackMessage(status),
    correlationId: typeof body.correlationId === 'string' ? body.correlationId : headerCorrelationId,
    errors: parseFieldErrors(body.errors),
  });
}

/**
 * פונקציית הקריאה המרכזית: כל פנייה לשרת עוברת כאן.
 * מוסיפה token כשהוא קיים, מפרקת שגיאות ל-ApiError ומטפלת בתגובות בלי גוף.
 */
export async function apiClient<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { method = 'GET', body, query, signal } = options;
  const token = sessionStore.getValidToken();

  const headers: Record<string, string> = { Accept: 'application/json' };
  if (body !== undefined) headers['Content-Type'] = 'application/json';
  if (token) headers.Authorization = `Bearer ${token}`;

  let response: Response;
  try {
    response = await fetch(buildUrl(path, query), {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
      signal,
    });
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') throw error;
    throw new ApiError({ status: NETWORK_ERROR_STATUS, message: fallbackMessage(NETWORK_ERROR_STATUS) });
  }

  if (response.ok) {
    if (response.status === 204) return undefined as T;
    return (await readJson(response)) as T;
  }

  // 401 על בקשה שנשלחה עם token פירושו שההתחברות אינה תקפה עוד.
  if (response.status === 401 && token) sessionStore.clear('unauthorized');

  throw toApiError(response, await readJson(response));
}
