import type { ChapterDto } from '../api/types';
import { toHebrewNumeral } from '../utils/hebrewNumerals';
import { TEHILLIM_BOOKS } from '../utils/tehillim';
import { cn } from '../utils/cn';
import { ChapterTile } from './ChapterTile';
import styles from './ChapterGrid.module.css';

interface ChapterGridProps {
  chapters: ChapterDto[];
  myChapters: ReadonlySet<number>;
  locked: boolean;
  availableOnly: boolean;
  onSelect: (number: number) => void;
}

export function ChapterGrid({ chapters, myChapters, locked, availableOnly, onSelect }: ChapterGridProps) {
  return (
    <div className={styles.sections}>
      {TEHILLIM_BOOKS.map((book) => {
        const inBook = chapters.filter((c) => c.number >= book.from && c.number <= book.to);
        const available = inBook.filter((c) => c.status !== 'Taken').length;
        const visible = availableOnly ? inBook.filter((c) => c.status !== 'Taken') : inBook;
        const headingId = `tehillim-book-${book.from}`;

        return (
          <section key={book.name} className={styles.section} aria-labelledby={headingId}>
            <header className={styles.sectionHeader}>
              <h3 id={headingId} className={styles.sectionTitle}>
                {book.name}
              </h3>
              <span className={styles.sectionRange}>
                פרקים {toHebrewNumeral(book.from)}–{toHebrewNumeral(book.to)}
              </span>
              <span className={cn(styles.sectionCount, available === 0 && styles.sectionDone)}>
                {available === 0 ? 'כל הפרקים נלקחו' : `${available} פנויים`}
              </span>
            </header>
            {visible.length > 0 ? (
              <ul className={styles.grid}>
                {visible.map((chapter) => (
                  <li key={chapter.number}>
                    <ChapterTile
                      chapter={chapter}
                      isMine={myChapters.has(chapter.number)}
                      locked={locked}
                      onSelect={onSelect}
                    />
                  </li>
                ))}
              </ul>
            ) : (
              <p className={styles.sectionEmpty}>אין פרקים פנויים בספר זה.</p>
            )}
          </section>
        );
      })}
    </div>
  );
}

export function ChapterGridSkeleton() {
  return (
    <ul className={styles.grid} aria-hidden="true">
      {Array.from({ length: 40 }, (_, index) => (
        <li key={index}>
          <span className={cn('skeleton', styles.tileSkeleton)} />
        </li>
      ))}
    </ul>
  );
}
