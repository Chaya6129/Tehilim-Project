import { ApiError, apiClient } from './client';
import type { AuthResponse, LoginRequest, RegisterRequest } from './types';

/** תגובת התחברות שבה ה-token קיים בוודאות. */
export type AuthResult = AuthResponse & { token: string };

function ensureToken(response: AuthResponse): AuthResult {
  if (!response.token) {
    throw new ApiError({ status: 500, message: 'התקבלה מהשרת תשובת התחברות לא תקינה.' });
  }
  return { ...response, token: response.token };
}

export async function register(body: RegisterRequest): Promise<AuthResult> {
  return ensureToken(await apiClient<AuthResponse>('/api/auth/register', { method: 'POST', body }));
}

export async function login(body: LoginRequest): Promise<AuthResult> {
  return ensureToken(await apiClient<AuthResponse>('/api/auth/login', { method: 'POST', body }));
}
