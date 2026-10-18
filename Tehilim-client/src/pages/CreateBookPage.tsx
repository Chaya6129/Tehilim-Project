import { useNavigate } from 'react-router-dom';
import { BookForm, type BookFormValues } from '../components/BookForm';
import { PageHeader } from '../components/PageHeader';
import { useToast } from '../components/toast/ToastContext';
import { useCreateBook } from '../hooks/useBooks';
import { useDocumentTitle } from '../hooks/useDocumentTitle';

const EMPTY_BOOK: BookFormValues = { goal: '', isPublic: true, categoryIds: [] };

export function CreateBookPage() {
  useDocumentTitle('פתיחת ספר חדש');
  const navigate = useNavigate();
  const { showToast } = useToast();
  const createBook = useCreateBook();

  const handleSubmit = (values: BookFormValues) => {
    createBook.mutate(values, {
      onSuccess: (book) => {
        showToast({ tone: 'success', message: 'הספר נפתח. אפשר לשתף את הקישור עם המשתתפים.' });
        navigate(`/books/${book.id}`, { replace: true });
      },
    });
  };

  return (
    <>
      <PageHeader
        title="פתיחת ספר חדש"
        description="ספר תהילים שלם, מאה וחמישים פרקים, לחלוקה בין המשתתפים."
      />
      <BookForm
        initialValues={EMPTY_BOOK}
        submitLabel="פתיחת הספר"
        isPending={createBook.isPending}
        error={createBook.error}
        onSubmit={handleSubmit}
        onCancel={() => navigate(-1)}
      />
    </>
  );
}
