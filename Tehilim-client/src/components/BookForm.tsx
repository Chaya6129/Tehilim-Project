import { useState, type FormEvent } from 'react';
import { useCategories } from '../hooks/useCategories';
import { describeError, fieldErrorsOf } from '../utils/errors';
import { cn } from '../utils/cn';
import { Button } from './Button';
import { FormAlert } from './TextField';
import formStyles from './Form.module.css';
import styles from './BookForm.module.css';

export interface BookFormValues {
  goal: string;
  isPublic: boolean;
  categoryIds: number[];
}

interface BookFormProps {
  initialValues: BookFormValues;
  submitLabel: string;
  isPending: boolean;
  error: unknown;
  onSubmit: (values: BookFormValues) => void;
  onCancel: () => void;
}

const GOAL_MAX = 200;

export function BookForm({ initialValues, submitLabel, isPending, error, onSubmit, onCancel }: BookFormProps) {
  const [values, setValues] = useState(initialValues);
  const [goalError, setGoalError] = useState<string | null>(null);
  const categories = useCategories();

  const serverErrors = fieldErrorsOf(error);
  const goalMessage = goalError ?? serverErrors.goal;

  const toggleCategory = (id: number) => {
    setValues((current) => ({
      ...current,
      categoryIds: current.categoryIds.includes(id)
        ? current.categoryIds.filter((value) => value !== id)
        : [...current.categoryIds, id],
    }));
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const goal = values.goal.trim();
    if (!goal) {
      setGoalError('יש לכתוב את יעד האמירה.');
      return;
    }
    setGoalError(null);
    onSubmit({ ...values, goal });
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit} noValidate>
      <FormAlert message={error ? describeError(error).message : null} />

      <div className={formStyles.field}>
        <label htmlFor="book-goal" className={formStyles.label}>
          יעד האמירה
        </label>
        <textarea
          id="book-goal"
          className={cn(formStyles.input, styles.goal, goalMessage && formStyles.invalid)}
          value={values.goal}
          onChange={(event) => setValues((current) => ({ ...current, goal: event.target.value }))}
          maxLength={GOAL_MAX}
          rows={3}
          placeholder="למשל: לרפואת פלוני בן פלונית"
          aria-invalid={goalMessage ? true : undefined}
          aria-describedby={goalMessage ? 'book-goal-error book-goal-count' : 'book-goal-count'}
        />
        <div className={styles.goalMeta}>
          {goalMessage && (
            <p id="book-goal-error" className={formStyles.error}>
              {goalMessage}
            </p>
          )}
          <span id="book-goal-count" className={styles.counter}>
            {values.goal.length}/{GOAL_MAX}
          </span>
        </div>
      </div>

      <fieldset className={styles.fieldset}>
        <legend className={formStyles.label}>מי יכול לראות את הספר</legend>
        <div className={styles.visibility}>
          <label className={cn(styles.option, values.isPublic && styles.optionSelected)}>
            <input
              type="radio"
              name="visibility"
              checked={values.isPublic}
              onChange={() => setValues((current) => ({ ...current, isPublic: true }))}
            />
            <span className={styles.optionTitle}>פומבי</span>
            <span className={styles.optionText}>יופיע ברשימת הספרים, וכל אחד יוכל להצטרף.</span>
          </label>
          <label className={cn(styles.option, !values.isPublic && styles.optionSelected)}>
            <input
              type="radio"
              name="visibility"
              checked={!values.isPublic}
              onChange={() => setValues((current) => ({ ...current, isPublic: false }))}
            />
            <span className={styles.optionTitle}>פרטי</span>
            <span className={styles.optionText}>לא יופיע ברשימה. רק מי שקיבל את הקישור יוכל להצטרף.</span>
          </label>
        </div>
      </fieldset>

      <fieldset className={styles.fieldset}>
        <legend className={formStyles.label}>קטגוריות</legend>
        {categories.isPending && <span className={cn('skeleton', styles.chipsSkeleton)} />}
        {categories.isError && (
          <p className={formStyles.error}>
            לא ניתן לטעון את הקטגוריות.{' '}
            <button type="button" className={styles.inlineButton} onClick={() => void categories.refetch()}>
              ניסיון נוסף
            </button>
          </p>
        )}
        {categories.data && categories.data.length === 0 && (
          <p className={formStyles.hint}>אין עדיין קטגוריות במערכת.</p>
        )}
        {categories.data && categories.data.length > 0 && (
          <div className={styles.chips}>
            {categories.data.map((category) => (
              <label key={category.id} className={styles.chip}>
                <input
                  type="checkbox"
                  checked={values.categoryIds.includes(category.id)}
                  onChange={() => toggleCategory(category.id)}
                />
                <span>{category.name}</span>
              </label>
            ))}
          </div>
        )}
        {serverErrors.categoryIds && <p className={formStyles.error}>{serverErrors.categoryIds}</p>}
      </fieldset>

      <div className={styles.actions}>
        <Button type="submit" isLoading={isPending}>
          {submitLabel}
        </Button>
        <Button variant="ghost" onClick={onCancel} disabled={isPending}>
          ביטול
        </Button>
      </div>
    </form>
  );
}
