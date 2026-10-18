import { useState, type FormEvent } from 'react';
import { useAuth } from '../auth/AuthContext';
import { useTakeChapter } from '../hooks/useChapters';
import { describeError, fieldErrorsOf, isApiError } from '../utils/errors';
import { toHebrewNumeral } from '../utils/hebrewNumerals';
import { Button } from './Button';
import { Modal } from './Modal';
import { FormAlert, TextField } from './TextField';
import { useToast } from './toast/ToastContext';
import styles from './TakeChapterDialog.module.css';

const TAKEN_BY_OTHER = 'הפרק נלקח זה עתה על ידי משתתף אחר, אנא בחר פרק אחר';

interface TakeChapterDialogProps {
  bookId: number;
  chapterNumber: number;
  onClose: () => void;
  onTaken: (number: number) => void;
}

export function TakeChapterDialog({ bookId, chapterNumber, onClose, onTaken }: TakeChapterDialogProps) {
  const { user } = useAuth();
  const { showToast, showError } = useToast();
  const takeChapter = useTakeChapter(bookId);
  const [guestName, setGuestName] = useState('');
  const hebrew = toHebrewNumeral(chapterNumber);

  const validationError = isApiError(takeChapter.error, 400) ? takeChapter.error : null;
  const fieldErrors = fieldErrorsOf(validationError);

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (takeChapter.isPending) return;

    takeChapter.mutate(
      { number: chapterNumber, guestName: user ? null : guestName.trim() || null },
      {
        onSuccess: () => {
          onTaken(chapterNumber);
          showToast({ tone: 'success', message: `פרק ${hebrew} נרשם עבורך. יישר כוח!` });
          onClose();
        },
        onError: (error) => {
          if (isApiError(error, 400)) return; // מוצג בתוך החלון, ליד השדה
          if (isApiError(error, 409)) {
            showToast({ tone: 'error', message: TAKEN_BY_OTHER });
          } else {
            showError(error, 'נטילת הפרק לא הצליחה.');
          }
          onClose();
        },
      },
    );
  };

  return (
    <Modal open title={`נטילת פרק ${hebrew}`} onClose={onClose} dismissible={!takeChapter.isPending}>
      <form onSubmit={handleSubmit} className={styles.form} noValidate>
        <p className={styles.chapterNumeral} aria-hidden="true">
          {hebrew}
        </p>

        {user ? (
          <p className={styles.lead}>
            הפרק יירשם על שם <strong>{user.fullName}</strong>, וכל המשתתפים יראו שהוא נלקח.
          </p>
        ) : (
          <>
            <p className={styles.lead}>הפרק יירשם לאמירה, וכל המשתתפים יראו שהוא נלקח.</p>
            <TextField
              label="השם שיוצג ליד הפרק (לא חובה)"
              value={guestName}
              onChange={(event) => setGuestName(event.target.value)}
              maxLength={100}
              autoComplete="name"
              placeholder="למשל: ישראל ישראלי"
              error={fieldErrors.guestName}
              autoFocus
            />
          </>
        )}

        <FormAlert
          message={validationError && !fieldErrors.guestName ? describeError(validationError).message : null}
        />

        <div className={styles.actions}>
          <Button type="submit" isLoading={takeChapter.isPending} autoFocus={Boolean(user)}>
            {takeChapter.isPending ? 'רושם את הפרק…' : 'נטילת הפרק'}
          </Button>
          <Button variant="ghost" onClick={onClose} disabled={takeChapter.isPending}>
            ביטול
          </Button>
        </div>
      </form>
    </Modal>
  );
}
