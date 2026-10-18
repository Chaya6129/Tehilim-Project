import { OrnamentDivider } from './Ornament';
import styles from './Footer.module.css';

export function Footer() {
  return (
    <footer className={styles.footer}>
      <OrnamentDivider />
      <p>חלוקת פרקי תהילים לאמירה משותפת</p>
    </footer>
  );
}
