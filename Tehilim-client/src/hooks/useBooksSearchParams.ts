import { useCallback, useMemo } from 'react';
import { useSearchParams } from 'react-router-dom';
import { BookSortBy } from '../api/types';
import { parsePositiveInt } from '../utils/parse';

export const SORT_OPTIONS = {
  newest: { label: 'החדשים ביותר', sortBy: BookSortBy.CreatedAt, descending: true },
  oldest: { label: 'הוותיקים ביותר', sortBy: BookSortBy.CreatedAt, descending: false },
  'most-available': { label: 'הכי הרבה פרקים פנויים', sortBy: BookSortBy.AvailableChapters, descending: true },
  'least-available': { label: 'קרובים להשלמה', sortBy: BookSortBy.AvailableChapters, descending: false },
} as const;

export type SortOption = keyof typeof SORT_OPTIONS;
const DEFAULT_SORT: SortOption = 'newest';

function isSortOption(value: string | null): value is SortOption {
  return value !== null && Object.prototype.hasOwnProperty.call(SORT_OPTIONS, value);
}

export interface BooksFilters {
  page: number;
  search: string;
  categoryId: number | undefined;
  sort: SortOption;
}

/** מצב הדפדוף, החיפוש, הסינון והמיון של רשימת הספרים, שמור ב-URL. */
export function useBooksSearchParams() {
  const [params, setParams] = useSearchParams();

  const filters = useMemo<BooksFilters>(() => {
    const sort = params.get('sort');
    return {
      page: parsePositiveInt(params.get('page')) ?? 1,
      search: params.get('q') ?? '',
      categoryId: parsePositiveInt(params.get('category')),
      sort: isSortOption(sort) ? sort : DEFAULT_SORT,
    };
  }, [params]);

  /** כל שינוי שאינו שינוי עמוד מחזיר לעמוד הראשון. */
  const update = useCallback(
    (changes: Partial<BooksFilters>, options: { replace?: boolean } = {}) => {
      setParams(
        (previous) => {
          const next = new URLSearchParams(previous);
          const apply = (key: string, value: string | undefined) => {
            if (value) next.set(key, value);
            else next.delete(key);
          };
          if ('search' in changes) apply('q', changes.search?.trim() || undefined);
          if ('categoryId' in changes) apply('category', changes.categoryId?.toString());
          if ('sort' in changes) apply('sort', changes.sort === DEFAULT_SORT ? undefined : changes.sort);
          const page = changes.page ?? 1;
          apply('page', page > 1 ? String(page) : undefined);
          return next;
        },
        { replace: options.replace },
      );
    },
    [setParams],
  );

  const hasActiveFilters = filters.search !== '' || filters.categoryId !== undefined;

  return { filters, update, hasActiveFilters };
}
