import { useCallback, useMemo, useRef, useState, type ReactNode } from 'react';
import { describeError } from '../../utils/errors';
import { cn } from '../../utils/cn';
import { ToastContext, type ToastContextValue, type ToastInput } from './ToastContext';
import styles from './Toast.module.css';

interface ToastItem extends ToastInput {
  id: number;
}

const MAX_VISIBLE = 4;
const DURATION_MS = { success: 4500, info: 6000, error: 8000 } as const;

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<ToastItem[]>([]);
  const nextId = useRef(1);

  const dismiss = useCallback((id: number) => {
    setToasts((items) => items.filter((item) => item.id !== id));
  }, []);

  const showToast = useCallback(
    (toast: ToastInput) => {
      const id = nextId.current++;
      setToasts((items) => [...items.slice(-(MAX_VISIBLE - 1)), { ...toast, id }]);
      window.setTimeout(() => dismiss(id), DURATION_MS[toast.tone]);
    },
    [dismiss],
  );

  const showError = useCallback(
    (error: unknown, fallback?: string) => {
      const { message, correlationId } = describeError(error, fallback);
      showToast({
        tone: 'error',
        message,
        detail: correlationId ? `מזהה לדיווח: ${correlationId}` : undefined,
      });
    },
    [showToast],
  );

  const value = useMemo<ToastContextValue>(() => ({ showToast, showError }), [showToast, showError]);

  return (
    <ToastContext.Provider value={value}>
      {children}
      <div className={styles.region} aria-live="polite">
        {toasts.map((toast) => (
          <div
            key={toast.id}
            className={cn(styles.toast, styles[toast.tone])}
            role={toast.tone === 'error' ? 'alert' : 'status'}
          >
            <span className={styles.mark} aria-hidden="true" />
            <div className={styles.content}>
              <p className={styles.message}>{toast.message}</p>
              {toast.detail && <p className={styles.detail}>{toast.detail}</p>}
            </div>
            <button
              type="button"
              className={styles.close}
              onClick={() => dismiss(toast.id)}
              aria-label="סגירת ההודעה"
            >
              ×
            </button>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  );
}
