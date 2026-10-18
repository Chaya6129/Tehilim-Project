import { Link, NavLink, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { cn } from '../utils/cn';
import { OrnamentBand, StarMark } from './Ornament';
import styles from './Header.module.css';

const AUTH_PATHS = ['/login', '/register'];

function navClass({ isActive }: { isActive: boolean }): string {
  return cn(styles.navLink, isActive && styles.active);
}

export function Header() {
  const { user, isAuthenticated, isAdmin, signOut } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const returnState = AUTH_PATHS.includes(location.pathname) ? undefined : { from: location };

  const handleSignOut = () => {
    signOut();
    navigate('/');
  };

  return (
    <header className={styles.header}>
      <div className={styles.inner}>
        <Link to="/" className={styles.brand}>
          <StarMark className={styles.brandMark} />
          <span className={styles.brandName}>חלוקת תהילים</span>
        </Link>

        <nav aria-label="ניווט ראשי" className={styles.nav}>
          <NavLink to="/" end className={navClass}>
            כל הספרים
          </NavLink>
          {isAuthenticated && (
            <NavLink to="/my-books" className={navClass}>
              הספרים שלי
            </NavLink>
          )}
          {isAdmin && (
            <NavLink to="/admin/categories" className={navClass}>
              קטגוריות
            </NavLink>
          )}
        </nav>

        <div className={styles.account}>
          {user ? (
            <>
              <span className={styles.greeting}>שלום, {user.fullName}</span>
              <button type="button" className={styles.accountLink} onClick={handleSignOut}>
                התנתקות
              </button>
            </>
          ) : (
            <>
              <Link to="/login" state={returnState} className={styles.accountLink}>
                התחברות
              </Link>
              <Link to="/register" state={returnState} className={styles.registerLink}>
                הרשמה
              </Link>
            </>
          )}
        </div>
      </div>
      <OrnamentBand className={styles.band} />
    </header>
  );
}
