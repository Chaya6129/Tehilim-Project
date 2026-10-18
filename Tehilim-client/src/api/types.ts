// טיפוסים התואמים ל-swagger.json של השרת.
// שדות שמוגדרים שם כ-nullable מוקלדים כאן כ-`T | null`.

export type BookStatus = 'Open' | 'Completed';
export type ChapterStatus = 'Available' | 'Taken';

/** BookSortBy נשלח כמספר (enum מספרי ב-swagger). */
export const BookSortBy = {
  CreatedAt: 0,
  AvailableChapters: 1,
} as const;
export type BookSortBy = (typeof BookSortBy)[keyof typeof BookSortBy];

export interface AuthResponse {
  token: string | null;
  expiresAtUtc: string;
  fullName: string | null;
  role: string | null;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface CategoryDto {
  id: number;
  name: string | null;
}

export interface CategoryRequest {
  name: string;
}

export interface ChapterDto {
  number: number;
  status: ChapterStatus | null;
  takenByName: string | null;
  takenAt: string | null;
}

export interface TakeChapterRequest {
  guestName?: string | null;
}

export interface BookListItemDto {
  id: number;
  goal: string | null;
  isPublic: boolean;
  status: BookStatus | null;
  ownerName: string | null;
  availableChapters: number;
  createdAt: string;
  categories: CategoryDto[] | null;
}

export interface BookDetailsDto {
  id: number;
  goal: string | null;
  isPublic: boolean;
  status: BookStatus | null;
  ownerId: number;
  ownerName: string | null;
  createdAt: string;
  completedAt: string | null;
  chapters: ChapterDto[] | null;
  categories: CategoryDto[] | null;
}

export interface BookListItemDtoPagedResult {
  items: BookListItemDto[] | null;
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface CreateBookRequest {
  goal: string;
  isPublic?: boolean;
  categoryIds?: number[] | null;
}

export interface UpdateBookRequest {
  goal: string;
  isPublic?: boolean;
  categoryIds?: number[] | null;
}

/** פרמטרי הדפדוף והסינון של GET /api/books, בצד הלקוח. */
export interface BooksQuery {
  page: number;
  pageSize: number;
  search?: string;
  categoryId?: number;
  sortBy?: BookSortBy;
  descending?: boolean;
}
