import type { ReactNode } from 'react';
import { StarMark } from './Ornament';
import styles from './AuthPanel.module.css';

interface AuthPanelProps {
  title: string;
  subtitle: string;
  children: ReactNode;
  footer: ReactNode;
}

export function AuthPanel({ title, subtitle, children, footer }: AuthPanelProps) {
  return (
    <section className={styles.panel}>
      <StarMark className={styles.mark} />
      <h1 className={styles.title}>{title}</h1>
      <p className={styles.subtitle}>{subtitle}</p>
      {children}
      <p className={styles.footer}>{footer}</p>
    </section>
  );
}
