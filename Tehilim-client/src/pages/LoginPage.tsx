import { useState, type FormEvent } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Link, Navigate, useLocation } from 'react-router-dom';
import { login } from '../api/auth';
import { useAuth } from '../auth/AuthContext';
import { getRedirectTarget } from '../auth/redirect';
import { AuthPanel } from '../components/AuthPanel';
import { Button } from '../components/Button';
import { FormAlert, TextField } from '../components/TextField';
import { useToast } from '../components/toast/ToastContext';
import { useDocumentTitle } from '../hooks/useDocumentTitle';
import { describeError, fieldErrorsOf, isApiError } from '../utils/errors';
import styles from './AuthPages.module.css';

interface LoginErrors {
  email?: string;
  password?: string;
}

function validate(email: string, password: string): LoginErrors {
  const errors: LoginErrors = {};
  if (!email.trim()) errors.email = 'יש להזין כתובת אימייל.';
  if (!password) errors.password = 'יש להזין סיסמה.';
  return errors;
}

export function LoginPage() {
  useDocumentTitle('התחברות');
  const { isAuthenticated, signIn } = useAuth();
  const { showToast } = useToast();
  const location = useLocation();
  const redirectTo = getRedirectTarget(location.state);

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [clientErrors, setClientErrors] = useState<LoginErrors>({});

  const mutation = useMutation({
    mutationFn: login,
    onSuccess: (result) => {
      signIn(result);
      showToast({ tone: 'success', message: `שלום, ${result.fullName ?? ''}. התחברת בהצלחה.` });
    },
  });

  // אחרי התחברות מוצלחת, החזרה לדף שממנו הגיע המשתמש.
  if (isAuthenticated) return <Navigate to={redirectTo} replace />;

  const serverErrors = fieldErrorsOf(mutation.error);
  const formMessage = mutation.error
    ? isApiError(mutation.error, 401)
      ? 'אימייל או סיסמה שגויים.'
      : describeError(mutation.error).message
    : null;

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const errors = validate(email, password);
    setClientErrors(errors);
    if (Object.keys(errors).length > 0 || mutation.isPending) return;
    mutation.mutate({ email: email.trim(), password });
  };

  return (
    <AuthPanel
      title="התחברות"
      subtitle="כדי לפתוח ספרים ולרשום פרקים על שמך."
      footer={
        <>
          עדיין אין לך חשבון?{' '}
          <Link to="/register" state={location.state}>
            להרשמה
          </Link>
        </>
      }
    >
      <form className={styles.form} onSubmit={handleSubmit} noValidate>
        <FormAlert message={formMessage} />
        <TextField
          label="אימייל"
          type="email"
          dir="ltr"
          autoComplete="email"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
          error={clientErrors.email ?? serverErrors.email}
          autoFocus
        />
        <TextField
          label="סיסמה"
          type="password"
          dir="ltr"
          autoComplete="current-password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          error={clientErrors.password ?? serverErrors.password}
        />
        <Button type="submit" isLoading={mutation.isPending} className={styles.submit}>
          התחברות
        </Button>
      </form>
    </AuthPanel>
  );
}
