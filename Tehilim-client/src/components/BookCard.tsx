import { Link } from 'react-router-dom';
import type { BookListItemDto } from '../api/types';
import { formatHebrewDate } from '../utils/date';
import { TOTAL_CHAPTERS } from '../utils/tehillim';
import { cn } from '../utils/cn';
import { Badge } from './Badge';
import { CategoryTags } from './CategoryTags';
import { ProgressBar } from './ProgressBar';
import styles from './BookCard.module.css';

interface BookCardProps {
  book: BookListItemDto;
  showVisibility?: boolean;
}

export function BookCard({ book, showVisibility = false }: BookCardProps) {
  const isCompleted = book.status === 'Completed';
  const taken = TOTAL_CHAPTERS - book.availableChapters;

  return (
    <article className={cn(styles.card, isCompleted && styles.completed)}>
      <div className={styles.top}>
        <CategoryTags categories={book.categories} />
        <div className={styles.badges}>
          {showVisibility && (
            <Badge tone={book.isPublic ? 'olive' : 'neutral'}>{book.isPublic ? 'פומבי' : 'פרטי'}</Badge>
          )}
          {isCompleted && <Badge tone="gold">הושלם</Badge>}
        </div>
      </div>

      <h3 className={styles.title}>
        <Link to={`/books/${book.id}`} className={styles.link}>
          {book.goal || 'ספר ללא יעד'}
        </Link>
      </h3>

      <p className={styles.meta}>
        <span>נפתח על ידי {book.ownerName || 'משתמש'}</span>
        <span className={styles.date}>{formatHebrewDate(book.createdAt)}</span>
      </p>

      <div className={styles.progress}>
        <ProgressBar taken={taken} />
        <p className={styles.counts}>
          {isCompleted ? (
            <span>כל {TOTAL_CHAPTERS} הפרקים נלקחו</span>
          ) : (
            <>
              <span>
                {taken} מתוך {TOTAL_CHAPTERS} נלקחו
              </span>
              <span className={styles.available}>{book.availableChapters} פנויים</span>
            </>
          )}
        </p>
      </div>
    </article>
  );
}

export function BookCardSkeleton() {
  return (
    <div className={styles.card} aria-hidden="true">
      <span className={cn('skeleton', styles.skeletonTag)} />
      <span className={cn('skeleton', styles.skeletonTitle)} />
      <span className={cn('skeleton', styles.skeletonLine)} />
      <span className={cn('skeleton', styles.skeletonBar)} />
    </div>
  );
}
