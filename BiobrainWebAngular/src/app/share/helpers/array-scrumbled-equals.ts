export function scrambledEquals<T>(list1: T[] | null | undefined, list2: T[] | null | undefined): boolean {
  if (list1 === null && list2 === null) {
    return true;
  }

  if (list1 === undefined && list2 === undefined) {
    return true;
  }

  const set1 = new Set<T>(list1);
  const set2 = new Set<T>(list2);

  if (set1.size !== set2.size) {
    return false;
  }

  for (const item of set1) {
    if (!set2.has(item)) {
      return false;
    }
  }

  return true;
}
