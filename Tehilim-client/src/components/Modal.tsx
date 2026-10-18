import { useEffect, useId, useRef, type MouseEvent, type ReactNode, type SyntheticEvent } from 'react';
import { StarMark } from './Ornament';
import styles from './Modal.module.css';

interface ModalProps {
  open: boolean;
  title: string;
  onClose: () => void;
  /** false חוסם סגירה (Esc, לחיצה ברקע), למשל בזמן שליחה. */
  dismissible?: boolean;
  children: ReactNode;
}

// מבוסס על <dialog> המקורי: לכידת פוקוס, Esc והחזרת הפוקוס מגיעים מהדפדפן.
export function Modal({ open, title, onClose, dismissible = true, children }: ModalProps) {
  const ref = useRef<HTMLDialogElement>(null);
  const titleId = useId();

  useEffect(() => {
    const dialog = ref.current;
    if (!dialog) return;
    if (open && !dialog.open) dialog.showModal();
    else if (!open && dialog.open) dialog.close();
  }, [open]);

  const handleCancel = (event: SyntheticEvent<HTMLDialogElement>) => {
    event.preventDefault();
    if (dismissible) onClose();
  };

  // הדפדפן עשוי לסגור את החלון בעצמו (למשל Esc כפול ב-Chrome). מסנכרנים את המצב עם ההורה.
  const handleNativeClose = () => {
    if (open) onClose();
  };

  const handleBackdropClick = (event: MouseEvent<HTMLDialogElement>) => {
    if (event.target === event.currentTarget && dismissible) onClose();
  };

  return (
    <dialog
      ref={ref}
      className={styles.dialog}
      aria-labelledby={titleId}
      onCancel={handleCancel}
      onClose={handleNativeClose}
      onClick={handleBackdropClick}
    >
      <div className={styles.panel}>
        <StarMark className={styles.mark} />
        <h2 id={titleId} className={styles.title}>
          {title}
        </h2>
        {children}
      </div>
    </dialog>
  );
}
