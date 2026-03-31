export function randomItemForArray<T>(array: T[]): T {
  if (array.length === 0) {
    throw new Error('Array has no elements.');
  }

  const index = Math.floor(Math.random() * array.length);
  return array[index];
}
