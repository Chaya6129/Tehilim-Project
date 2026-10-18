import { memo } from 'react';
import type { ChapterDto } from '../api/types';
import { formatDateTime } from '../utils/date';
import { toHebrewNumeral } from '../utils/hebrewNumerals';
import { cn } from '../utils/cn';
import styles from './ChapterGrid.module.css';

interface ChapterTileProps {
  chapter: ChapterDto;
  isMine: boolean;
  locked: boolean;
  onSelect: (number: number) => void;
}

function CheckGlyph() {
  return (
    <svg className={styles.check} viewBox="0 0 12 12" aria-hidden="true" focusable="false">
      <path d="M2.5 6.4l2.3 2.3 4.7-5" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round" />
    </svg>
  );
}

export const ChapterTile = memo(function ChapterTile({ chapter, isMine, locked, onSelect }: ChapterTileProps) {
  const hebrew = toHebrewNumeral(chapter.number);
  const numerals = (
    <>
      <span className={styles.hebrew} aria-hidden="true">
        {hebrew}
      </span>
      <span className={styles.number} aria-hidden="true">
        {chapter.number}
      </span>
    </>
  );

  if (chapter.status === 'Taken') {
    const name = chapter.takenByName || 'משתתף';
    const takenAt = chapter.takenAt ? formatDateTime(chapter.takenAt) : '';
    const owner = isMine ? 'על ידך' : `על ידי ${name}`;
    return (
      <div
        className={cn(styles.tile, styles.taken, isMine && styles.mine)}
        title={takenAt ? `נלקח ${owner}, ${takenAt}` : `נלקח ${owner}`}
      >
        {numerals}
        <span className={styles.takenBy} aria-hidden="true">
          <CheckGlyph />
          <span className={styles.takenName}>{isMine ? 'שלך' : name}</span>
        </span>
        <span className="visually-hidden">
          פרק {hebrew} ({chapter.number}), נלקח {owner}
        </span>
      </div>
    );
  }

  return (
    <button
      type="button"
      className={cn(styles.tile, styles.available)}
      onClick={() => onSelect(chapter.number)}
      disabled={locked}
      aria-label={`פרק ${hebrew} (${chapter.number}), פנוי. בחירה לנטילת הפרק`}
    >
      {numerals}
      <span className={styles.freeLabel} aria-hidden="true">
        פנוי
      </span>
    </button>
  );
});
