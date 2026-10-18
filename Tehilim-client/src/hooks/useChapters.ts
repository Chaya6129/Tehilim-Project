import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { getChapters, takeChapter } from '../api/chapters';
import { queryKeys } from '../api/queryKeys';
import type { BookDetailsDto, ChapterDto } from '../api/types';
import { isApiError } from '../utils/errors';

const POLL_INTERVAL_MS = 5000;

/**
 * פרקי הספר, עם רענון אוטומטי כל כמה שניות וברגע שהחלון חוזר לפוקוס.
 * הנתונים הראשוניים נלקחים מהספר שכבר נטען (GET /api/books/{id} כולל את הפרקים).
 */
export function useChapters(bookId: number, options: { enabled: boolean; polling: boolean }) {
  const queryClient = useQueryClient();
  return useQuery({
    queryKey: queryKeys.books.chapters(bookId),
    queryFn: ({ signal }) => getChapters(bookId, signal),
    enabled: options.enabled,
    initialData: () =>
      queryClient.getQueryData<BookDetailsDto>(queryKeys.books.detail(bookId))?.chapters ?? undefined,
    initialDataUpdatedAt: () => queryClient.getQueryState(queryKeys.books.detail(bookId))?.dataUpdatedAt,
    staleTime: 0,
    refetchInterval: options.polling ? POLL_INTERVAL_MS : false,
    refetchIntervalInBackground: false,
    refetchOnWindowFocus: 'always',
  });
}

interface TakeChapterVariables {
  number: number;
  guestName: string | null;
}

export function useTakeChapter(bookId: number) {
  const queryClient = useQueryClient();
  const refreshBook = () => queryClient.invalidateQueries({ queryKey: queryKeys.books.detail(bookId) });

  return useMutation({
    mutationFn: ({ number, guestName }: TakeChapterVariables) =>
      takeChapter(bookId, number, guestName ? { guestName } : {}),
    onSuccess: (chapter) => {
      queryClient.setQueryData<ChapterDto[]>(queryKeys.books.chapters(bookId), (chapters) =>
        chapters?.map((item) => (item.number === chapter.number ? chapter : item)),
      );
      void refreshBook();
      void queryClient.invalidateQueries({ queryKey: queryKeys.books.lists() });
      void queryClient.invalidateQueries({ queryKey: queryKeys.books.mine() });
    },
    onError: (error) => {
      // 409: מישהו אחר הקדים. רענון מיידי כדי שהרשת תציג את המצב האמיתי.
      if (isApiError(error, 409)) void refreshBook();
    },
  });
}
