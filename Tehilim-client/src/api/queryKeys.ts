import type { BooksQuery } from './types';

// מפתחות היררכיים: ביטול תוקף של books.detail(id) מרענן גם את פרקי הספר.
export const queryKeys = {
  books: {
    all: ['books'] as const,
    lists: () => ['books', 'list'] as const,
    list: (query: BooksQuery) => ['books', 'list', query] as const,
    mine: () => ['books', 'mine'] as const,
    detail: (id: number) => ['books', 'detail', id] as const,
    chapters: (id: number) => ['books', 'detail', id, 'chapters'] as const,
  },
  categories: ['categories'] as const,
};
