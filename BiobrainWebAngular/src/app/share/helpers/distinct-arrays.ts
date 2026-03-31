type Comparator<T> = (x1: T, x2: T) => boolean;

export function distinct<T>(
  arr: T[],
  comparator?: Comparator<T> | undefined
): T[] {
  const a = arr.concat();
  const comparer = comparator ?? ((x1: T, x2: T) => x1 === x2);

  for (let i = 0; i < a.length; ++i) {
    for (let j = i + 1; j < a.length; ++j) {
      if (comparer(a[i], a[j])) {
        a.splice(j--, 1);
      }
    }
  }

  return a;
}
