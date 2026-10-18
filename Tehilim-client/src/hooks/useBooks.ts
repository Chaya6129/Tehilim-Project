import { keepPreviousData, useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { createBook, deleteBook, getBook, getBooks, getMyBooks, updateBook } from '../api/books';
import { queryKeys } from '../api/queryKeys';
import type { BooksQuery, CreateBookRequest, UpdateBookRequest } from '../api/types';

export function useBooksList(query: BooksQuery) {
  return useQuery({
    queryKey: queryKeys.books.list(query),
    queryFn: ({ signal }) => getBooks(query, signal),
    placeholderData: keepPreviousData,
  });
}

export function useMyBooks() {
  return useQuery({
    queryKey: queryKeys.books.mine(),
    queryFn: ({ signal }) => getMyBooks(signal),
  });
}

export function useBook(id: number) {
  return useQuery({
    queryKey: queryKeys.books.detail(id),
    queryFn: ({ signal }) => getBook(id, signal),
  });
}

export function useCreateBook() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateBookRequest) => createBook(body),
    onSuccess: (book) => {
      queryClient.setQueryData(queryKeys.books.detail(book.id), book);
      void queryClient.invalidateQueries({ queryKey: queryKeys.books.lists() });
      void queryClient.invalidateQueries({ queryKey: queryKeys.books.mine() });
    },
  });
}

export function useUpdateBook(id: number) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: UpdateBookRequest) => updateBook(id, body),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.books.all });
    },
  });
}

export function useDeleteBook(id: number) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => deleteBook(id),
    // את פרטי הספר לא מרעננים: הוא נמחק, ורענון היה מחזיר 404 לדף שעוד מוצג.
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.books.lists() });
      void queryClient.invalidateQueries({ queryKey: queryKeys.books.mine() });
    },
  });
}
