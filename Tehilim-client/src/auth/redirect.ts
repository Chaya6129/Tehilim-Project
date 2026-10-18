/** הנתיב שאליו חוזרים אחרי התחברות, מתוך location.state שהועבר מ-RequireAuth. */
export function getRedirectTarget(state: unknown): string {
  if (typeof state !== 'object' || state === null || !('from' in state)) return '/';
  const from: unknown = state.from;
  if (typeof from !== 'object' || from === null || !('pathname' in from)) return '/';
  const { pathname } = from;
  if (typeof pathname !== 'string' || !pathname.startsWith('/')) return '/';
  if (pathname === '/login' || pathname === '/register') return '/';
  const search = 'search' in from && typeof from.search === 'string' ? from.search : '';
  return `${pathname}${search}`;
}
