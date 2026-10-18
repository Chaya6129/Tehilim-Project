import type { CategoryDto } from '../api/types';
import styles from './CategoryTags.module.css';

export function CategoryTags({ categories }: { categories: CategoryDto[] | null }) {
  const named = (categories ?? []).filter((category) => category.name);
  if (named.length === 0) return null;
  return (
    <ul className={styles.list} aria-label="קטגוריות">
      {named.map((category) => (
        <li key={category.id} className={styles.tag}>
          {category.name}
        </li>
      ))}
    </ul>
  );
}
