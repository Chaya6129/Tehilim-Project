import { useState, type FormEvent } from 'react';
import type { CategoryDto } from '../api/types';
import { Button } from '../components/Button';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { EmptyState } from '../components/EmptyState';
import { ErrorState } from '../components/ErrorState';
import { PageHeader } from '../components/PageHeader';
import { TextField } from '../components/TextField';
import { useToast } from '../components/toast/ToastContext';
import {
  useCategories,
  useCreateCategory,
  useDeleteCategory,
  useUpdateCategory,
} from '../hooks/useCategories';
import { useDocumentTitle } from '../hooks/useDocumentTitle';
import { describeError, fieldErrorsOf, isApiError } from '../utils/errors';
import styles from './AdminCategoriesPage.module.css';

const NAME_MAX = 50;

/** הודעת שגיאה לשדה השם: 409 (שם כפול), פירוט ולידציה, או ההודעה הכללית. */
function nameErrorOf(error: unknown): string | undefined {
  if (!error) return undefined;
  if (isApiError(error, 409)) return error.message;
  return fieldErrorsOf(error).name ?? describeError(error).message;
}

export function AdminCategoriesPage() {
  useDocumentTitle('ניהול קטגוריות');
  const categories = useCategories();
  const [pendingDelete, setPendingDelete] = useState<CategoryDto | null>(null);
  const deleteCategory = useDeleteCategory();
  const { showToast, showError } = useToast();

  const handleDelete = () => {
    if (!pendingDelete) return;
    deleteCategory.mutate(pendingDelete.id, {
      onSuccess: () => showToast({ tone: 'success', message: `הקטגוריה "${pendingDelete.name ?? ''}" נמחקה.` }),
      onError: (error) => showError(error, 'מחיקת הקטגוריה לא הצליחה.'),
      onSettled: () => setPendingDelete(null),
    });
  };

  return (
    <>
      <PageHeader title="ניהול קטגוריות" description="הקטגוריות שמהן בוחרים בעת פתיחת ספר, ושלפיהן מסננים את הרשימה." />

      <div className={styles.layout}>
        <AddCategoryForm />

        <section aria-labelledby="categories-heading" className={styles.listSection}>
          <h2 id="categories-heading" className={styles.heading}>
            קטגוריות קיימות
          </h2>

          {categories.isPending && <span className="skeleton" style={{ height: '12rem' }} aria-hidden="true" />}
          {categories.isError && <ErrorState error={categories.error} onRetry={() => void categories.refetch()} />}
          {categories.data && categories.data.length === 0 && (
            <EmptyState title="אין עדיין קטגוריות" description="הקטגוריה הראשונה שתתווסף תופיע כאן." />
          )}
          {categories.data && categories.data.length > 0 && (
            <ul className={styles.list}>
              {categories.data.map((category) => (
                <CategoryRow key={category.id} category={category} onDelete={() => setPendingDelete(category)} />
              ))}
            </ul>
          )}
        </section>
      </div>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="מחיקת קטגוריה"
        message={`הקטגוריה "${pendingDelete?.name ?? ''}" תוסר מהרשימה. לא ניתן לבטל פעולה זו.`}
        confirmLabel="מחיקת הקטגוריה"
        isPending={deleteCategory.isPending}
        onConfirm={handleDelete}
        onCancel={() => setPendingDelete(null)}
      />
    </>
  );
}

function AddCategoryForm() {
  const [name, setName] = useState('');
  const [clientError, setClientError] = useState<string | null>(null);
  const createCategory = useCreateCategory();
  const { showToast } = useToast();

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const trimmed = name.trim();
    if (!trimmed) {
      setClientError('יש להזין שם לקטגוריה.');
      return;
    }
    setClientError(null);
    createCategory.mutate(trimmed, {
      onSuccess: (category) => {
        setName('');
        createCategory.reset();
        showToast({ tone: 'success', message: `הקטגוריה "${category.name ?? trimmed}" נוספה.` });
      },
    });
  };

  return (
    <form className={styles.addForm} onSubmit={handleSubmit} noValidate>
      <h2 className={styles.heading}>הוספת קטגוריה</h2>
      <TextField
        label="שם הקטגוריה"
        value={name}
        onChange={(event) => setName(event.target.value)}
        maxLength={NAME_MAX}
        error={clientError ?? nameErrorOf(createCategory.error)}
      />
      <Button type="submit" isLoading={createCategory.isPending}>
        הוספה
      </Button>
    </form>
  );
}

function CategoryRow({ category, onDelete }: { category: CategoryDto; onDelete: () => void }) {
  const [isEditing, setIsEditing] = useState(false);
  const [name, setName] = useState(category.name ?? '');
  const [clientError, setClientError] = useState<string | null>(null);
  const updateCategory = useUpdateCategory();
  const { showToast } = useToast();

  const startEditing = () => {
    setName(category.name ?? '');
    setClientError(null);
    updateCategory.reset();
    setIsEditing(true);
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const trimmed = name.trim();
    if (!trimmed) {
      setClientError('יש להזין שם לקטגוריה.');
      return;
    }
    if (trimmed === category.name) {
      setIsEditing(false);
      return;
    }
    setClientError(null);
    updateCategory.mutate(
      { id: category.id, name: trimmed },
      {
        onSuccess: () => {
          setIsEditing(false);
          showToast({ tone: 'success', message: 'שם הקטגוריה עודכן.' });
        },
      },
    );
  };

  if (isEditing) {
    return (
      <li className={styles.row}>
        <form className={styles.editForm} onSubmit={handleSubmit} noValidate>
          <TextField
            label={`שם חדש לקטגוריה "${category.name ?? ''}"`}
            value={name}
            onChange={(event) => setName(event.target.value)}
            maxLength={NAME_MAX}
            error={clientError ?? nameErrorOf(updateCategory.error)}
            autoFocus
          />
          <div className={styles.rowActions}>
            <Button type="submit" size="sm" isLoading={updateCategory.isPending}>
              שמירה
            </Button>
            <Button variant="ghost" size="sm" onClick={() => setIsEditing(false)} disabled={updateCategory.isPending}>
              ביטול
            </Button>
          </div>
        </form>
      </li>
    );
  }

  return (
    <li className={styles.row}>
      <span className={styles.name}>{category.name}</span>
      <div className={styles.rowActions}>
        <Button variant="ghost" size="sm" onClick={startEditing} aria-label={`שינוי שם הקטגוריה ${category.name ?? ''}`}>
          שינוי שם
        </Button>
        <Button variant="danger" size="sm" onClick={onDelete} aria-label={`מחיקת הקטגוריה ${category.name ?? ''}`}>
          מחיקה
        </Button>
      </div>
    </li>
  );
}
