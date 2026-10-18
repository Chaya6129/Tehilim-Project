import { useEffect, useRef, useState } from 'react';
import { useDebouncedValue } from './useDebouncedValue';

const DEBOUNCE_MS = 350;

/**
 * שדה חיפוש מקומי שמסתנכרן עם ערך חיצוני (ה-URL):
 * ההקלדה נשלחת אחרי debounce, ושינוי חיצוני (כפתור "אחורה", ניקוי סינון) מעדכן את השדה.
 */
export function useSearchInput(committedValue: string, onCommit: (value: string) => void) {
  const [text, setText] = useState(committedValue);
  const debounced = useDebouncedValue(text, DEBOUNCE_MS);
  const lastCommitted = useRef(committedValue);
  const onCommitRef = useRef(onCommit);
  onCommitRef.current = onCommit;

  useEffect(() => {
    if (debounced.trim() === lastCommitted.current) return;
    lastCommitted.current = debounced.trim();
    onCommitRef.current(debounced);
  }, [debounced]);

  useEffect(() => {
    if (committedValue === lastCommitted.current) return;
    lastCommitted.current = committedValue;
    setText(committedValue);
  }, [committedValue]);

  return [text, setText] as const;
}
