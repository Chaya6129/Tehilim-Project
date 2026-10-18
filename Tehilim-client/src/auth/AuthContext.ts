import { createContext, useContext } from 'react';
import type { AuthResult } from '../api/auth';

export interface AuthUser {
  fullName: string;
  role: string;
  /** מזהה המשתמש מתוך ה-JWT. null אם לא ניתן היה לחלץ אותו. */
  userId: number | null;
}

export interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  signIn: (result: AuthResult) => void;
  signOut: () => void;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

export function useAuth(): AuthContextValue {
  const value = useContext(AuthContext);
  if (!value) throw new Error('useAuth חייב לפעול בתוך AuthProvider');
  return value;
}
