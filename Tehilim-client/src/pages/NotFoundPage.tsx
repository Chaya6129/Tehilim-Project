import { Link } from 'react-router-dom';
import { buttonClass } from '../components/Button';
import { StarMark } from '../components/Ornament';
import { useDocumentTitle } from '../hooks/useDocumentTitle';
import styles from './NotFoundPage.module.css';

export function NotFoundPage() {
  useDocumentTitle('הדף לא נמצא');
  return (
    <section className={styles.page}>
      <div className={styles.seal} aria-hidden="true">
        <StarMark className={styles.star} />
        <span className={styles.code}>404</span>
      </div>
      <h1 className={styles.title}>הדף לא נמצא</h1>
      <p className={styles.text}>ייתכן שהכתובת שגויה, או שהדף הוסר.</p>
      <Link to="/" className={buttonClass('primary')}>
        לרשימת הספרים
      </Link>
    </section>
  );
}
