import { QueryClient } from '@tanstack/react-query';
import { ApiError } from './client';

function isClientError(error: unknown): boolean {
  return error instanceof ApiError && error.status >= 400 && error.status < 500;
}

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      // אין טעם לנסות שוב שגיאות 4xx (404, 403 וכו'). שגיאות רשת ו-5xx מקבלות עוד שני ניסיונות.
      retry: (failureCount, error) => !isClientError(error) && failureCount < 2,
    },
    mutations: {
      retry: false,
    },
  },
});
