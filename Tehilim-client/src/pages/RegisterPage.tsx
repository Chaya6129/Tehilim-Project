import { useState, type FormEvent } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Link, Navigate, useLocation } from 'react-router-dom';
import { register } from '../api/auth';
import { useAuth } from '../auth/AuthContext';
import { getRedirectTarget } from '../auth/redirect';
import { AuthPanel } from '../components/AuthPanel';
import { Button } from '../components/Button';
import { FormAlert, TextField } from '../components/TextField';
import { useToast } from '../components/toast/ToastContext';
import { useDocumentTitle } from '../hooks/useDocumentTitle';
import { describeError, fieldErrorsOf, isApiError } from '../utils/errors';
import styles from './AuthPages.module.css';

const EMAIL_TAKEN = 'כתובת האימייל כבר רשומה במערכת.';
const PASSWORD_MIN = 6;

interface RegisterValues {
  fullName: string;
  email: string;
  password: string;
}

type RegisterErrors = Partial<Record<keyof RegisterValues, string>>;

function validate(values: RegisterValues): RegisterErrors {
  const errors: RegisterErrors = {};
  if (!values.fullName.trim()) errors.fullName = 'יש להזין שם מלא.';
  if (!values.email.trim()) errors.email = 'יש להזין כתובת אימייל.';
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(values.email.trim())) errors.email = 'כתובת האימייל אינה תקינה.';
  if (values.password.length < PASSWORD_MIN) errors.password = `הסיסמה צריכה לכלול לפחות ${PASSWORD_MIN} תווים.`;
  return errors;
}

export function RegisterPage() {
  useDocumentTitle('הרשמה');
  const { isAuthenticated, signIn } = useAuth();
  const { showToast } = useToast();
  const location = useLocation();
  const redirectTo = getRedirectTarget(location.state);

  const [values, setValues] = useState<RegisterValues>({ fullName: '', email: '', password: '' });
  const [clientErrors, setClientErrors] = useState<RegisterErrors>({});

  const mutation = useMutation({
    mutationFn: register,
    onSuccess: (result) => {
      signIn(result);
      showToast({ tone: 'success', message: `ברוך הבא, ${result.fullName ?? ''}. החשבון נוצר בהצלחה.` });
    },
  });

  if (isAuthenticated) return <Navigate to={redirectTo} replace />;

  const isEmailTaken = isApiError(mutation.error, 409);
  const serverErrors = fieldErrorsOf(mutation.error);
  const formMessage = mutation.error
    ? isEmailTaken
      ? EMAIL_TAKEN
      : describeError(mutation.error).message
    : null;

  const errorFor = (field: keyof RegisterValues) => clientErrors[field] ?? serverErrors[field];

  const setField = (field: keyof RegisterValues) => (value: string) =>
    setValues((current) => ({ ...current, [field]: value }));

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const errors = validate(values);
    setClientErrors(errors);
    if (Object.keys(errors).length > 0 || mutation.isPending) return;
    mutation.mutate({ fullName: values.fullName.trim(), email: values.email.trim(), password: values.password });
  };

  return (
    <AuthPanel
      title="הרשמה"
      subtitle="חשבון אחד לפתיחת ספרים ולמעקב אחר הפרקים שלך."
      footer={
        <>
          כבר יש לך חשבון?{' '}
          <Link to="/login" state={location.state}>
            להתחברות
          </Link>
        </>
      }
    >
      <form className={styles.form} onSubmit={handleSubmit} noValidate>
        <FormAlert message={formMessage} />
        <TextField
          label="שם מלא"
          autoComplete="name"
          maxLength={100}
          value={values.fullName}
          onChange={(event) => setField('fullName')(event.target.value)}
          error={errorFor('fullName')}
          autoFocus
        />
        <TextField
          label="אימייל"
          type="email"
          dir="ltr"
          autoComplete="email"
          maxLength={200}
          value={values.email}
          onChange={(event) => setField('email')(event.target.value)}
          error={errorFor('email') ?? (isEmailTaken ? EMAIL_TAKEN : undefined)}
        />
        <TextField
          label="סיסמה"
          type="password"
          dir="ltr"
          autoComplete="new-password"
          maxLength={100}
          value={values.password}
          onChange={(event) => setField('password')(event.target.value)}
          hint={`לפחות ${PASSWORD_MIN} תווים.`}
          error={errorFor('password')}
        />
        <Button type="submit" isLoading={mutation.isPending} className={styles.submit}>
          יצירת חשבון
        </Button>
      </form>
    </AuthPanel>
  );
}
