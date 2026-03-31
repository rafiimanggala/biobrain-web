export function alphaNumericStringComparator(first = '', second = ''): number {
  return first.localeCompare(second, 'en', { numeric: true });
}
