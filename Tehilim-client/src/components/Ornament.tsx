import { useId } from 'react';
import { cn } from '../utils/cn';
import styles from './Ornament.module.css';

interface OrnamentProps {
  className?: string;
}

/** כוכב שמונה קצוות: שני ריבועים משולבים ועיגול במרכז. */
export function StarMark({ className }: OrnamentProps) {
  return (
    <svg className={cn(styles.star, className)} viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <g fill="none" stroke="currentColor" strokeWidth="1.2" strokeLinejoin="round">
        <rect x="5.5" y="5.5" width="13" height="13" />
        <rect x="5.5" y="5.5" width="13" height="13" transform="rotate(45 12 12)" />
        <circle cx="12" cy="12" r="2.6" />
      </g>
    </svg>
  );
}

/** קו מפריד עם כוכב במרכז ויהלומים משני צדדיו. */
export function OrnamentDivider({ className }: OrnamentProps) {
  return (
    <svg className={cn(styles.divider, className)} viewBox="0 0 240 24" aria-hidden="true" focusable="false">
      <g fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
        <path d="M6 12H90M150 12H234" />
        <path d="M95 12l4-4 4 4-4 4zM137 12l4-4 4 4-4 4z" />
        <rect x="113" y="5" width="14" height="14" />
        <rect x="113" y="5" width="14" height="14" transform="rotate(45 120 12)" />
      </g>
      <circle cx="120" cy="12" r="2.2" fill="currentColor" />
    </svg>
  );
}

/** רצועת עיטור חוזרת: יהלומים ונקודות. */
export function OrnamentBand({ className }: OrnamentProps) {
  const patternId = `band-${useId().replace(/[^a-zA-Z0-9-]/g, '')}`;
  return (
    <svg className={cn(styles.band, className)} aria-hidden="true" focusable="false">
      <defs>
        <pattern id={patternId} width="22" height="10" patternUnits="userSpaceOnUse">
          <path d="M1 5L6 1L11 5L6 9Z" fill="none" stroke="currentColor" strokeWidth="0.9" />
          <circle cx="16.5" cy="5" r="1.1" fill="currentColor" />
        </pattern>
      </defs>
      <rect width="100%" height="100%" fill={`url(#${patternId})`} />
    </svg>
  );
}
