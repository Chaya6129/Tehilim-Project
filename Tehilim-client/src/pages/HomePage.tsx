import { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { BookCard, BookCardSkeleton } from '../components/BookCard';
import { Button, buttonClass } from '../components/Button';
import { EmptyState } from '../components/EmptyState';
import { ErrorState } from '../components/ErrorState';
import { OrnamentDivider } from '../components/Ornament';
import { Pagination } from '../components/Pagination';
import { SORT_OPTIONS, useBooksSearchParams, type SortOption } from '../hooks/useBooksSearchParams';
import { useBooksList } from '../hooks/useBooks';
import { useCategories } from '../hooks/useCategories';
import { useDocumentTitle } from '../hooks/useDocumentTitle';
import { useSearchInput } from '../hooks/useSearchInput';
import { parsePositiveInt } from '../utils/parse';
import { cn } from '../utils/cn';
import formStyles from '../components/Form.module.css';
import styles from './HomePage.module.css';

const PAGE_SIZE = 12;
const SORT_ENTRIES = Object.entries(SORT_OPTIONS) as Array<[SortOption, (typeof SORT_OPTIONS)[SortOption]]>;

export function HomePage() {
  useDocumentTitle(null);
  const { isAuthenticated } = useAuth();
  const { filters, update, hasActiveFilters } = useBooksSearchParams();
  const categories = useCategories();
  const [searchText, setSearchText] = useSearchInput(filters.search, (search) =>
    update({ search }, { replace: true }),
  );

  const sort = SORT_OPTIONS[filters.sort];
  const books = useBooksList({
    page: filters.page,
    pageSize: PAGE_SIZE,
    search: filters.search || undefined,
    categoryId: filters.categoryId,
    sortBy: sort.sortBy,
    descending: sort.descending,
  });

  const totalCount = books.data?.totalCount ?? 0;
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));
  const items = books.data?.items ?? [];

  // עמוד שחורג מהטווח (למשל אחרי מחיקה או קישור ישן) מתוקן לעמוד האחרון.
  useEffect(() => {
    if (books.data && totalCount > 0 && filters.page > totalPages) {
      update({ page: totalPages }, { replace: true });
    }
  }, [books.data, totalCount, totalPages, filters.page, update]);

  const changePage = (page: number) => {
    update({ page });
    document.getElementById('books')?.scrollIntoView({ block: 'start' });
  };

  const newBookLink = isAuthenticated ? (
    <Link to="/books/new" className={buttonClass('primary')}>
      פתיחת ספר חדש
    </Link>
  ) : (
    <Link to="/login" state={{ from: { pathname: '/books/new' } }} className={buttonClass('secondary')}>
      התחברות לפתיחת ספר
    </Link>
  );

  return (
    <>
      <section className={styles.hero}>
        <OrnamentDivider />
        <h1 className={styles.heroTitle}>ספר תהילים, פרק אחר פרק</h1>
        <p className={styles.heroText}>
          בוחרים ספר, נוטלים פרק פנוי ואומרים אותו. כשכל מאה וחמישים הפרקים נלקחים, הספר הושלם.
        </p>
        <div className={styles.heroAction}>{newBookLink}</div>
      </section>

      <section id="books" className={styles.listSection} aria-labelledby="books-heading">
        <h2 id="books-heading" className="visually-hidden">
          ספרים פתוחים
        </h2>

        <div className={styles.toolbar} role="search">
          <div className={formStyles.field}>
            <label htmlFor="books-search" className={formStyles.label}>
              חיפוש לפי יעד
            </label>
            <input
              id="books-search"
              type="search"
              className={formStyles.input}
              value={searchText}
              onChange={(event) => setSearchText(event.target.value)}
              placeholder="למשל: רפואה, הצלחה, עילוי נשמה"
              maxLength={100}
            />
          </div>

          <div className={formStyles.field}>
            <label htmlFor="books-category" className={formStyles.label}>
              קטגוריה
            </label>
            <select
              id="books-category"
              className={formStyles.input}
              value={filters.categoryId ?? ''}
              onChange={(event) => update({ categoryId: parsePositiveInt(event.target.value) })}
            >
              <option value="">כל הקטגוריות</option>
              {categories.data?.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </select>
          </div>

          <div className={formStyles.field}>
            <label htmlFor="books-sort" className={formStyles.label}>
              מיון
            </label>
            <select
              id="books-sort"
              className={formStyles.input}
              value={filters.sort}
              onChange={(event) => {
                const value = event.target.value;
                const option = SORT_ENTRIES.find(([key]) => key === value);
                if (option) update({ sort: option[0] });
              }}
            >
              {SORT_ENTRIES.map(([key, option]) => (
                <option key={key} value={key}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        {books.data && totalCount > 0 && (
          <p className={styles.summary} aria-live="polite">
            {totalCount === 1 ? 'נמצא ספר אחד' : `נמצאו ${totalCount} ספרים`}
          </p>
        )}

        {books.isPending && (
          <div className={styles.grid} aria-busy="true">
            {Array.from({ length: 6 }, (_, index) => (
              <BookCardSkeleton key={index} />
            ))}
          </div>
        )}

        {books.isError && !books.data && <ErrorState error={books.error} onRetry={() => void books.refetch()} />}

        {books.data && items.length === 0 && (
          hasActiveFilters ? (
            <EmptyState
              title="לא נמצאו ספרים מתאימים"
              description="נסו מילת חיפוש אחרת או קטגוריה אחרת."
              action={
                <Button variant="secondary" onClick={() => update({ search: '', categoryId: undefined })}>
                  ניקוי החיפוש והסינון
                </Button>
              }
            />
          ) : (
            <EmptyState
              title="עדיין אין ספרים פתוחים"
              description="אפשר לפתוח ספר ראשון ולשתף אותו עם המשתתפים."
              action={newBookLink}
            />
          )
        )}

        {items.length > 0 && (
          <>
            <div className={cn(styles.grid, books.isPlaceholderData && styles.stale)}>
              {items.map((book) => (
                <BookCard key={book.id} book={book} />
              ))}
            </div>
            <Pagination page={filters.page} totalPages={totalPages} onChange={changePage} />
          </>
        )}
      </section>
    </>
  );
}
