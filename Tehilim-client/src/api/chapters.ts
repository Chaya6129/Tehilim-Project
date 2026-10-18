import { apiClient } from './client';
import type { ChapterDto, TakeChapterRequest } from './types';

export function getChapters(bookId: number, signal?: AbortSignal): Promise<ChapterDto[]> {
  return apiClient<ChapterDto[]>(`/api/books/${bookId}/chapters`, { signal });
}

export function takeChapter(bookId: number, number: number, body: TakeChapterRequest): Promise<ChapterDto> {
  return apiClient<ChapterDto>(`/api/books/${bookId}/chapters/${number}`, { method: 'PATCH', body });
}
