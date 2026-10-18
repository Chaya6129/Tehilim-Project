import { createContext, useContext } from 'react';

export type ToastTone = 'success' | 'error' | 'info';

export interface ToastInput {
  tone: ToastTone;
  message: string;
  /** שורה משנית קטנה, למשל מזהה לדיווח. */
  detail?: string;
}

export interface ToastContextValue {
  showToast: (toast: ToastInput) => void;
  /** הודעת שגיאה מתוך ApiError. בשגיאות שרת מצורף גם ה-correlationId. */
  showError: (error: unknown, fallback?: string) => void;
}

export const ToastContext = createContext<ToastContextValue | null>(null);

export function useToast(): ToastContextValue {
  const value = useContext(ToastContext);
  if (!value) throw new Error('useToast חייב לפעול בתוך ToastProvider');
  return value;
}
