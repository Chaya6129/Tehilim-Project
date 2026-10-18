import { ApiError } from '../api/client';

export function isApiError(error: unknown, status?: number): error is ApiError {
  return error instanceof ApiError && (status === undefined || error.status === status);
}

export interface ErrorDescription {
  message: string;
  /** מוצג רק בשגיאות שרת (5xx), לצורך דיווח. */
  correlationId: string | null;
}

export function describeError(error: unknown, fallback = 'אירעה שגיאה לא צפויה.'): ErrorDescription {
  if (error instanceof ApiError) {
    return {
      message: error.message || fallback,
      correlationId: error.status >= 500 ? error.correlationId : null,
    };
  }
  return { message: fallback, correlationId: null };
}

/** הודעות השגיאה של כל שדה, מאוחדות למחרוזת אחת לתצוגה. */
export function fieldErrorsOf(error: unknown): Record<string, string> {
  if (!(error instanceof ApiError) || !error.errors) return {};
  const result: Record<string, string> = {};
  for (const [field, messages] of Object.entries(error.errors)) {
    result[field] = messages.join(' ');
  }
  return result;
}
