import { Link } from 'react-router-dom';
import { BookCard, BookCardSkeleton } from '../components/BookCard';
import { buttonClass } from '../components/Button';
import { EmptyState } from '../components/EmptyState';
import { ErrorState } from '../components/ErrorState';
import { PageHeader } from '../components/PageHeader';
import { useMyBooks } from '../hooks/useBooks';
import { useDocumentTitle } from '../hooks/useDocumentTitle';
import styles from './ListPages.module.css';

export function MyBooksPage() {
  useDocumentTitle('הספרים שלי');
  const books = useMyBooks();

  const newBookLink = (
    <Link to="/books/new" className={buttonClass('primary')}>
      פתיחת ספר חדש
    </Link>
  );

  return (
    <>
      <PageHeader
        title="הספרים שלי"
        description="כל הספרים שפתחת, כולל ספרים פרטיים שנגישים בקישור בלבד."
        actions={newBookLink}
      />

      {books.isPending && (
        <div className={styles.grid} aria-busy="true">
          {Array.from({ length: 3 }, (_, index) => (
            <BookCardSkeleton key={index} />
          ))}
        </div>
      )}

      {books.isError && <ErrorState error={books.error} onRetry={() => void books.refetch()} />}

      {books.data && books.data.length === 0 && (
        <EmptyState
          title="עדיין לא פתחת ספר"
          description="פותחים ספר עם יעד לאמירה, ומשתפים את הקישור עם המשתתפים."
          action={newBookLink}
        />
      )}

      {books.data && books.data.length > 0 && (
        <div className={styles.grid}>
          {books.data.map((book) => (
            <BookCard key={book.id} book={book} showVisibility />
          ))}
        </div>
      )}
    </>
  );
}
