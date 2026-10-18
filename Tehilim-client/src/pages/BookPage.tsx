import { useEffect, useMemo, useState } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { queryKeys } from '../api/queryKeys';
import type { BookDetailsDto } from '../api/types';
import { useAuth } from '../auth/AuthContext';
import { BookHeader, BookHeaderSkeleton } from '../components/BookHeader';
import { buttonClass } from '../components/Button';
import { ChapterGrid, ChapterGridSkeleton } from '../components/ChapterGrid';
import { CompletedBanner } from '../components/CompletedBanner';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { EmptyState } from '../components/EmptyState';
import { ErrorState } from '../components/ErrorState';
import { TakeChapterDialog } from '../components/TakeChapterDialog';
import { useToast } from '../components/toast/ToastContext';
import { useBook, useDeleteBook } from '../hooks/useBooks';
import { useChapters } from '../hooks/useChapters';
import { useDocumentTitle } from '../hooks/useDocumentTitle';
import { copyText } from '../utils/clipboard';
import { isApiError } from '../utils/errors';
import { parsePositiveInt } from '../utils/parse';
import { TOTAL_CHAPTERS } from '../utils/tehillim';
import { cn } from '../utils/cn';
import styles from './BookPage.module.css';

export function BookPage() {
  const { id } = useParams();
  const bookId = parsePositiveInt(id);
  if (bookId === undefined) return <BookNotFound />;
  return <BookView key={bookId} bookId={bookId} />;
}

function BookNotFound() {
  useDocumentTitle('הספר לא נמצא');
  return (
    <EmptyState
      title="הספר לא נמצא"
      description="ייתכן שהקישור שגוי, או שהספר נמחק."
      action={
        <Link to="/" className={buttonClass('secondary')}>
          לרשימת הספרים
        </Link>
      }
    />
  );
}

function BookView({ bookId }: { bookId: number }) {
  const book = useBook(bookId);
  useDocumentTitle(book.data?.goal);

  if (book.isPending) {
    return (
      <>
        <BookHeaderSkeleton />
        <ChapterGridSkeleton />
      </>
    );
  }
  if (book.isError) {
    if (isApiError(book.error, 404)) return <BookNotFound />;
    return <ErrorState error={book.error} onRetry={() => void book.refetch()} />;
  }
  return <BookContent book={book.data} />;
}

function BookContent({ book }: { book: BookDetailsDto }) {
  const { user, isAdmin } = useAuth();
  const { showToast, showError } = useToast();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [selected, setSelected] = useState<number | null>(null);
  const [myChapters, setMyChapters] = useState<ReadonlySet<number>>(() => new Set());
  const [availableOnly, setAvailableOnly] = useState(false);
  const [confirmingDelete, setConfirmingDelete] = useState(false);
  const deleteBook = useDeleteBook(book.id);

  const serverCompleted = book.status === 'Completed';
  const chaptersQuery = useChapters(book.id, { enabled: true, polling: !serverCompleted });

  const chapters = useMemo(
    () => [...(chaptersQuery.data ?? book.chapters ?? [])].sort((a, b) => a.number - b.number),
    [chaptersQuery.data, book.chapters],
  );
  const takenCount = chapters.filter((chapter) => chapter.status === 'Taken').length;
  const allTaken = chapters.length >= TOTAL_CHAPTERS && takenCount === chapters.length;
  const isCompleted = serverCompleted || allTaken;

  // הרשת גילתה שכל הפרקים נלקחו: מביאים את הספר מחדש, כדי לקבל את הסטטוס ואת תאריך ההשלמה.
  useEffect(() => {
    if (allTaken && !serverCompleted) {
      void queryClient.invalidateQueries({ queryKey: queryKeys.books.detail(book.id), exact: true });
    }
  }, [allTaken, serverCompleted, book.id, queryClient]);

  const isOwner = user !== null && user.userId !== null && user.userId === book.ownerId;
  const canDelete = isOwner || isAdmin;

  const handleShare = async () => {
    const url = `${window.location.origin}/books/${book.id}`;
    if (await copyText(url)) {
      showToast({ tone: 'success', message: 'הקישור הועתק. אפשר לשלוח אותו למשתתפים.' });
    } else {
      showToast({ tone: 'error', message: 'לא ניתן היה להעתיק אוטומטית. אפשר להעתיק את הקישור ידנית:', detail: url });
    }
  };

  const handleDelete = () => {
    deleteBook.mutate(undefined, {
      onSuccess: () => {
        showToast({ tone: 'success', message: 'הספר נמחק.' });
        navigate(isOwner ? '/my-books' : '/', { replace: true });
      },
      onError: (error) => {
        setConfirmingDelete(false);
        showError(error, 'מחיקת הספר לא הצליחה.');
      },
    });
  };

  const handleTaken = (number: number) => {
    setMyChapters((current) => new Set(current).add(number));
  };

  return (
    <>
      <BookHeader
        book={book}
        takenCount={takenCount}
        isCompleted={isCompleted}
        canEdit={isOwner}
        canDelete={canDelete}
        onShare={() => void handleShare()}
        onDelete={() => setConfirmingDelete(true)}
      />

      {isCompleted && <CompletedBanner completedAt={book.completedAt} />}

      <section className={styles.chapters} aria-labelledby="chapters-heading">
        <div className={styles.bar}>
          <div className={styles.barTitle}>
            <h2 id="chapters-heading" className={styles.heading}>
              פרקי הספר
            </h2>
            {!isCompleted && (
              <p className={styles.hint}>
                {chaptersQuery.isFetching && <span className={styles.pulse} aria-hidden="true" />}
                בוחרים פרק פנוי כדי ליטול אותו. הרשימה מתעדכנת מעצמה.
              </p>
            )}
          </div>

          <ul className={styles.legend} aria-label="מקרא">
            <li>
              <span className={cn(styles.swatch, styles.swatchAvailable)} aria-hidden="true" />
              פנוי
            </li>
            <li>
              <span className={cn(styles.swatch, styles.swatchTaken)} aria-hidden="true" />
              נלקח
            </li>
            {myChapters.size > 0 && (
              <li>
                <span className={cn(styles.swatch, styles.swatchTaken, styles.swatchMine)} aria-hidden="true" />
                נלקח על ידך
              </li>
            )}
          </ul>

          {!isCompleted && (
            <label className={styles.toggle}>
              <input
                type="checkbox"
                checked={availableOnly}
                onChange={(event) => setAvailableOnly(event.target.checked)}
              />
              הצגת הפרקים הפנויים בלבד
            </label>
          )}
        </div>

        {chaptersQuery.isError && chapters.length > 0 && (
          <p className={styles.warning} role="status">
            לא ניתן לרענן את רשימת הפרקים כרגע. המצב המוצג עשוי שלא להיות עדכני.
          </p>
        )}

        {chapters.length > 0 ? (
          <ChapterGrid
            chapters={chapters}
            myChapters={myChapters}
            locked={isCompleted}
            availableOnly={availableOnly && !isCompleted}
            onSelect={setSelected}
          />
        ) : chaptersQuery.isError ? (
          <ErrorState error={chaptersQuery.error} onRetry={() => void chaptersQuery.refetch()} />
        ) : (
          <ChapterGridSkeleton />
        )}
      </section>

      {selected !== null && !isCompleted && (
        <TakeChapterDialog
          key={selected}
          bookId={book.id}
          chapterNumber={selected}
          onClose={() => setSelected(null)}
          onTaken={handleTaken}
        />
      )}

      <ConfirmDialog
        open={confirmingDelete}
        title="מחיקת הספר"
        message="הספר וכל הפרקים שנלקחו בו יימחקו. לא ניתן לבטל פעולה זו."
        confirmLabel="מחיקת הספר"
        isPending={deleteBook.isPending}
        onConfirm={handleDelete}
        onCancel={() => setConfirmingDelete(false)}
      />
    </>
  );
}
