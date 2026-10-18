export const TOTAL_CHAPTERS = 150;

export interface TehillimBook {
  name: string;
  from: number;
  to: number;
}

/** חמשת הספרים שבספר תהילים. */
export const TEHILLIM_BOOKS: readonly TehillimBook[] = [
  { name: 'ספר ראשון', from: 1, to: 41 },
  { name: 'ספר שני', from: 42, to: 72 },
  { name: 'ספר שלישי', from: 73, to: 89 },
  { name: 'ספר רביעי', from: 90, to: 106 },
  { name: 'ספר חמישי', from: 107, to: 150 },
];
