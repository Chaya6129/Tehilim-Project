import { formatHebrewDate } from '../utils/date';
import { OrnamentDivider } from './Ornament';
import styles from './CompletedBanner.module.css';

export function CompletedBanner({ completedAt }: { completedAt: string | null }) {
  const date = completedAt ? formatHebrewDate(completedAt) : '';
  return (
    <section className={styles.banner} role="status">
      <OrnamentDivider className={styles.ornament} />
      <h2 className={styles.title}>הספר הושלם</h2>
      <p className={styles.text}>
        כל מאה וחמישים הפרקים נלקחו לאמירה{date ? `, ב${date}` : ''}. יישר כוח לכל המשתתפים.
      </p>
    </section>
  );
}
