const UNITS = ['', 'א', 'ב', 'ג', 'ד', 'ה', 'ו', 'ז', 'ח', 'ט'] as const;
const TENS = ['', 'י', 'כ', 'ל', 'מ', 'נ', 'ס', 'ע', 'פ', 'צ'] as const;
const HUNDREDS = ['', 'ק', 'ר', 'ש', 'ת'] as const;

const GERESH = '׳';
const GERSHAYIM = '״';

/**
 * המרת מספר (1–999) לכתיב עברי: 1 → "א", 15 → "ט״ו", 16 → "ט״ז", 150 → "ק״נ".
 * אות בודדת נכתבת בלי סימן; בצירוף של כמה אותיות מופיעים גרשיים לפני האות האחרונה.
 * עם withGeresh: true, אות בודדת מקבלת גרש ("א׳").
 */
export function toHebrewNumeral(value: number, options: { withGeresh?: boolean } = {}): string {
  if (!Number.isInteger(value) || value < 1 || value > 999) {
    throw new RangeError(`toHebrewNumeral: ${value} אינו מספר שלם בטווח 1–999`);
  }

  let letters = '';
  let rest = value;

  let hundreds = Math.floor(rest / 100);
  rest %= 100;
  while (hundreds >= 4) {
    letters += HUNDREDS[4];
    hundreds -= 4;
  }
  letters += HUNDREDS[hundreds] ?? '';

  // ט״ו וט״ז במקום צירופים שהם שמות ה׳.
  if (rest === 15 || rest === 16) {
    letters += `ט${UNITS[rest - 9] ?? ''}`;
  } else {
    letters += (TENS[Math.floor(rest / 10)] ?? '') + (UNITS[rest % 10] ?? '');
  }

  if (letters.length === 1) {
    return options.withGeresh ? `${letters}${GERESH}` : letters;
  }
  return `${letters.slice(0, -1)}${GERSHAYIM}${letters.slice(-1)}`;
}
