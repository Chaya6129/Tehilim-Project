import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { EmptyState } from '../components/EmptyState';
import { useAuth } from './AuthContext';

// ההסתרה כאן נועדה לנוחות בלבד. ההרשאה האמיתית נאכפת בשרת.
export function RequireAdmin() {
  const { isAuthenticated, isAdmin } = useAuth();
  const location = useLocation();
  if (!isAuthenticated) return <Navigate to="/login" replace state={{ from: location }} />;
  if (!isAdmin) {
    return (
      <EmptyState
        title="הדף מיועד למנהלי המערכת"
        description="אין לחשבון שלך הרשאה לצפות בדף זה."
      />
    );
  }
  return <Outlet />;
}
