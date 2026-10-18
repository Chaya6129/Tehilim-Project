import { describeError } from '../utils/errors';
import { Button } from './Button';
import styles from './ErrorState.module.css';

interface ErrorStateProps {
  error: unknown;
  title?: string;
  onRetry?: () => void;
}

export function ErrorState({ error, title = 'הטעינה לא הצליחה', onRetry }: ErrorStateProps) {
  const { message, correlationId } = describeError(error);
  return (
    <section className={styles.error} role="alert">
      <h2 className={styles.title}>{title}</h2>
      <p className={styles.message}>{message}</p>
      {correlationId && <p className={styles.correlation}>מזהה לדיווח: {correlationId}</p>}
      {onRetry && (
        <Button variant="secondary" onClick={onRetry}>
          ניסיון נוסף
        </Button>
      )}
    </section>
  );
}
