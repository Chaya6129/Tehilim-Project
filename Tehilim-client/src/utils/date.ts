import { toHebrewNumeral } from './hebrewNumerals';

// השרת שולח תאריכים ב-UTC. אם מחרוזת מגיעה בלי סימון אזור זמן (DateTime של .NET מסוג Unspecified),
// היא מתפרשת כאן כ-UTC, ולא כשעה מקומית כפי שהדפדפן היה מניח.
const HAS_TIME_ZONE = /(?:Z|[+-]\d{2}:?\d{2})$/i;

export function parseUtcDate(value: string): Date {
  return new Date(HAS_TIME_ZONE.test(value) ? value : `${value}Z`);
}

function toValidDate(value: string): Date | null {
  const date = parseUtcDate(value);
  return Number.isNaN(date.getTime()) ? null : date;
}

const hebrewCalendar = new Intl.DateTimeFormat('he-IL-u-ca-hebrew', {
  day: 'numeric',
  month: 'long',
  year: 'numeric',
});

const shortDateTime = new Intl.DateTimeFormat('he-IL', {
  day: 'numeric',
  month: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
});

/** תאריך עברי באותיות לפי השעון המקומי, למשל "ז׳ בחשוון תשפ״ז". */
export function formatHebrewDate(value: string): string {
  const date = toValidDate(value);
  if (!date) return '';
  const parts = hebrewCalendar.formatToParts(date);
  const day = Number(parts.find((part) => part.type === 'day')?.value);
  const month = parts.find((part) => part.type === 'month')?.value;
  const year = Number(parts.find((part) => part.type === 'year')?.value);
  if (!Number.isInteger(day) || !month || !Number.isInteger(year) || year % 1000 === 0) {
    return hebrewCalendar.format(date);
  }
  return `${toHebrewNumeral(day, { withGeresh: true })} ב${month} ${toHebrewNumeral(year % 1000)}`;
}

/** תאריך ושעה מקוצרים, לפי השעון המקומי של המשתמש. */
export function formatDateTime(value: string): string {
  const date = toValidDate(value);
  return date ? shortDateTime.format(date) : '';
}
