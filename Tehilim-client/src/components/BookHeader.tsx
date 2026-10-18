import { Link } from 'react-router-dom';
import type { BookDetailsDto } from '../api/types';
import { formatHebrewDate } from '../utils/date';
import { TOTAL_CHAPTERS } from '../utils/tehillim';
import { Badge } from './Badge';
import { Button, buttonClass } from './Button';
import { CategoryTags } from './CategoryTags';
import { ProgressBar } from './ProgressBar';
import styles from './BookHeader.module.css';

interface BookHeaderProps {
  book: BookDetailsDto;
  takenCount: number;
  isCompleted: boolean;
  canEdit: boolean;
  canDelete: boolean;
  onShare: () => void;
  onDelete: () => void;
}

export function BookHeader({ book, takenCount, isCompleted, canEdit, canDelete, onShare, onDelete }: BookHeaderProps) {
  const available = TOTAL_CHAPTERS - takenCount;
  return (
    <header className={styles.header}>
      <div className={styles.topRow}>
        <CategoryTags categories={book.categories} />
        <div className={styles.badges}>
          {!book.isPublic && <Badge>ספר פרטי, נגיש בקישור בלבד</Badge>}
          {isCompleted && <Badge tone="gold">הושלם</Badge>}
        </div>
      </div>

      <h1 className={styles.goal}>{book.goal || 'ספר ללא יעד'}</h1>

      <p className={styles.meta}>
        <span>נפתח על ידי {book.ownerName || 'משתמש'}</span>
        <span className={styles.date}>{formatHebrewDate(book.createdAt)}</span>
      </p>

      <div className={styles.progress}>
        <ProgressBar taken={takenCount} />
        <p className={styles.counts}>
          <span>
            <strong className={styles.countNumber}>{takenCount}</strong> מתוך {TOTAL_CHAPTERS} פרקים נלקחו
          </span>
          {!isCompleted && <span className={styles.available}>{available} פנויים</span>}
        </p>
      </div>

      <div className={styles.actions}>
        <Button variant="secondary" size="sm" onClick={onShare}>
          העתקת קישור לשיתוף
        </Button>
        {canEdit && (
          <Link to={`/books/${book.id}/edit`} className={buttonClass('ghost', 'sm')}>
            עריכת הספר
          </Link>
        )}
        {canDelete && (
          <Button variant="danger" size="sm" onClick={onDelete}>
            מחיקת הספר
          </Button>
        )}
      </div>
    </header>
  );
}

export function BookHeaderSkeleton() {
  return (
    <div className={styles.header} aria-hidden="true">
      <span className="skeleton" style={{ width: '25%', height: '1.25rem' }} />
      <span className="skeleton" style={{ width: '70%', height: '2.5rem' }} />
      <span className="skeleton" style={{ width: '40%', height: '1rem' }} />
      <span className="skeleton" style={{ width: '100%', height: '0.5rem' }} />
    </div>
  );
}
