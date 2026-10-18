import { apiClient } from './client';
import type { CategoryDto, CategoryRequest } from './types';

export function getCategories(signal?: AbortSignal): Promise<CategoryDto[]> {
  return apiClient<CategoryDto[]>('/api/categories', { signal });
}

export function createCategory(body: CategoryRequest): Promise<CategoryDto> {
  return apiClient<CategoryDto>('/api/categories', { method: 'POST', body });
}

export function updateCategory(id: number, body: CategoryRequest): Promise<void> {
  return apiClient<void>(`/api/categories/${id}`, { method: 'PUT', body });
}

export function deleteCategory(id: number): Promise<void> {
  return apiClient<void>(`/api/categories/${id}`, { method: 'DELETE' });
}
