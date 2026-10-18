import { Link, useNavigate, useParams } from 'react-router-dom';
import type { BookDetailsDto } from '../api/types';
import { useAuth } from '../auth/AuthContext';
import { BookForm, type BookFormValues } from '../components/BookForm';
import { buttonClass } from '../components/Button';
import { EmptyState } from '../components/EmptyState';
import { ErrorState } from '../components/ErrorState';
import { PageHeader } from '../components/PageHeader';
import { useToast } from '../components/toast/ToastContext';
import { useBook, useUpdateBook } from '../hooks/useBooks';
import { useDocumentTitle } from '../hooks/useDocumentTitle';
import { isApiError } from '../utils/errors';
import { parsePositiveInt } from '../utils/parse';
import { NotFoundPage } from './NotFoundPage';

export function EditBookPage() {
  useDocumentTitle('עריכת ספר');
  const { id } = useParams();
  const bookId = parsePositiveInt(id);
  if (bookId === undefined) return <NotFoundPage />;
  return <EditBookLoader key={bookId} bookId={bookId} />;
}

function EditBookLoader({ bookId }: { bookId: number }) {
  const { user } = useAuth();
  const book = useBook(bookId);

  if (book.isPending) return <span className="skeleton" style={{ height: '20rem' }} aria-hidden="true" />;
  if (book.isError) {
    if (isApiError(book.error, 404)) return <NotFoundPage />;
    return <ErrorState error={book.error} onRetry={() => void book.refetch()} />;
  }
  // ההרשאה האמיתית נבדקת בשרת (403). כאן רק חוסכים למשתמש טופס שלא יוכל לשמור.
  if (user?.userId !== book.data.ownerId) {
    return (
      <EmptyState
        title="רק מי שפתח את הספר יכול לערוך אותו"
        action={
          <Link to={`/books/${bookId}`} className={buttonClass('secondary')}>
            חזרה לספר
          </Link>
        }
      />
    );
  }
  return <EditBookForm book={book.data} />;
}

function EditBookForm({ book }: { book: BookDetailsDto }) {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const updateBook = useUpdateBook(book.id);

  const initialValues: BookFormValues = {
    goal: book.goal ?? '',
    isPublic: book.isPublic,
    categoryIds: (book.categories ?? []).map((category) => category.id),
  };

  const handleSubmit = (values: BookFormValues) => {
    updateBook.mutate(values, {
      onSuccess: () => {
        showToast({ tone: 'success', message: 'השינויים נשמרו.' });
        navigate(`/books/${book.id}`);
      },
    });
  };

  return (
    <>
      <PageHeader title="עריכת ספר" description="שינוי היעד, הפומביות והקטגוריות. הפרקים שנלקחו אינם משתנים." />
      <BookForm
        initialValues={initialValues}
        submitLabel="שמירת השינויים"
        isPending={updateBook.isPending}
        error={updateBook.error}
        onSubmit={handleSubmit}
        onCancel={() => navigate(`/books/${book.id}`)}
      />
    </>
  );
}
