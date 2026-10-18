import { TOTAL_CHAPTERS } from '../utils/tehillim';
import { cn } from '../utils/cn';
import styles from './ProgressBar.module.css';

interface ProgressBarProps {
  taken: number;
  className?: string;
}

export function ProgressBar({ taken, className }: ProgressBarProps) {
  const clamped = Math.min(Math.max(taken, 0), TOTAL_CHAPTERS);
  const percent = (clamped / TOTAL_CHAPTERS) * 100;
  return (
    <div
      className={cn(styles.track, className)}
      role="progressbar"
      aria-valuemin={0}
      aria-valuemax={TOTAL_CHAPTERS}
      aria-valuenow={clamped}
      aria-label={`${clamped} מתוך ${TOTAL_CHAPTERS} פרקים נלקחו`}
    >
      <div className={styles.fill} style={{ width: `${percent}%` }} />
    </div>
  );
}
