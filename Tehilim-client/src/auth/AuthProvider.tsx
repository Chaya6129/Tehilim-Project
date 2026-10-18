import { useCallback, useEffect, useMemo, useSyncExternalStore, type ReactNode } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import type { AuthResult } from '../api/auth';
import { queryKeys } from '../api/queryKeys';
import { useToast } from '../components/toast/ToastContext';
import { AuthContext, type AuthContextValue } from './AuthContext';
import { sessionFromAuthResult, sessionStore } from './sessionStore';

export function AuthProvider({ children }: { children: ReactNode }) {
  const session = useSyncExternalStore(sessionStore.subscribe, sessionStore.getSnapshot);
  const queryClient = useQueryClient();
  const { showToast } = useToast();

  useEffect(
    () =>
      sessionStore.onLogout((reason) => {
        queryClient.removeQueries({ queryKey: queryKeys.books.mine() });
        if (reason === 'expired') {
          showToast({ tone: 'info', message: 'תוקף ההתחברות הסתיים. יש להתחבר מחדש.' });
        } else if (reason === 'unauthorized') {
          showToast({ tone: 'error', message: 'ההתחברות אינה תקפה עוד. יש להתחבר מחדש.' });
        }
      }),
    [queryClient, showToast],
  );

  const signIn = useCallback(
    (result: AuthResult) => {
      sessionStore.set(sessionFromAuthResult(result));
      void queryClient.invalidateQueries();
    },
    [queryClient],
  );

  const signOut = useCallback(() => sessionStore.clear('manual'), []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user: session
        ? { fullName: session.fullName, role: session.role, userId: session.userId }
        : null,
      isAuthenticated: session !== null,
      isAdmin: session?.role === 'Admin',
      signIn,
      signOut,
    }),
    [session, signIn, signOut],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
