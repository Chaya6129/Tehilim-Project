import { apiClient } from './client';
import type {
  BookDetailsDto,
  BookListItemDto,
  BookListItemDtoPagedResult,
  BooksQuery,
  CreateBookRequest,
  UpdateBookRequest,
} from './types';

export function getBooks(query: BooksQuery, signal?: AbortSignal): Promise<BookListItemDtoPagedResult> {
  return apiClient<BookListItemDtoPagedResult>('/api/books', {
    query: {
      Page: query.page,
      PageSize: query.pageSize,
      Search: query.search,
      CategoryId: query.categoryId,
      SortBy: query.sortBy,
      Descending: query.descending,
    },
    signal,
  });
}

export function getMyBooks(signal?: AbortSignal): Promise<BookListItemDto[]> {
  return apiClient<BookListItemDto[]>('/api/books/mine', { signal });
}

export function getBook(id: number, signal?: AbortSignal): Promise<BookDetailsDto> {
  return apiClient<BookDetailsDto>(`/api/books/${id}`, { signal });
}

export function createBook(body: CreateBookRequest): Promise<BookDetailsDto> {
  return apiClient<BookDetailsDto>('/api/books', { method: 'POST', body });
}

export function updateBook(id: number, body: UpdateBookRequest): Promise<void> {
  return apiClient<void>(`/api/books/${id}`, { method: 'PUT', body });
}

export function deleteBook(id: number): Promise<void> {
  return apiClient<void>(`/api/books/${id}`, { method: 'DELETE' });
}
