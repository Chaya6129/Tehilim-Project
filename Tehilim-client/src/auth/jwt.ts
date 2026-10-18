const NAME_IDENTIFIER_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
// גיבוי: JwtSecurityTokenHandler ממפה כברירת מחדל את ה-claim הארוך לשם המקוצר "nameid".
const FALLBACK_CLAIMS = ['nameid', 'sub'] as const;

/** פענוח ה-payload של JWT (base64url), בלי אימות חתימה. האימות נעשה בשרת. */
export function decodeJwtPayload(token: string): Record<string, unknown> | null {
  const segment = token.split('.')[1];
  if (!segment) return null;
  try {
    const base64 = segment.replace(/-/g, '+').replace(/_/g, '/');
    const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');
    const binary = atob(padded);
    const bytes = Uint8Array.from(binary, (char) => char.charCodeAt(0));
    const parsed: unknown = JSON.parse(new TextDecoder().decode(bytes));
    return typeof parsed === 'object' && parsed !== null && !Array.isArray(parsed)
      ? (parsed as Record<string, unknown>)
      : null;
  } catch {
    return null;
  }
}

/** מזהה המשתמש מתוך ה-token, כמספר, או null אם אינו קיים או אינו מספר. */
export function getUserIdFromToken(token: string): number | null {
  const payload = decodeJwtPayload(token);
  if (!payload) return null;
  for (const claim of [NAME_IDENTIFIER_CLAIM, ...FALLBACK_CLAIMS]) {
    const raw = payload[claim];
    const value = Array.isArray(raw) ? raw[0] : raw;
    if (typeof value === 'string' || typeof value === 'number') {
      const id = Number(value);
      if (Number.isSafeInteger(id)) return id;
    }
  }
  return null;
}
