import { Button } from './Button';
import { Modal } from './Modal';
import styles from './ConfirmDialog.module.css';

interface ConfirmDialogProps {
  open: boolean;
  title: string;
  message: string;
  confirmLabel: string;
  isPending: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

export function ConfirmDialog({
  open,
  title,
  message,
  confirmLabel,
  isPending,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  return (
    <Modal open={open} title={title} onClose={onCancel} dismissible={!isPending}>
      <p className={styles.message}>{message}</p>
      <div className={styles.actions}>
        <Button variant="danger" onClick={onConfirm} isLoading={isPending}>
          {confirmLabel}
        </Button>
        <Button variant="ghost" onClick={onCancel} disabled={isPending}>
          ביטול
        </Button>
      </div>
    </Modal>
  );
}
