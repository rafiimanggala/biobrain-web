export function getScoreColor(progress: number): string {
  const lowerLimit = 49;
  const upperLimit = 69;

  if (Number.isNaN(progress)) return 'red';
  if (progress <= lowerLimit) return 'red';
  if (progress > lowerLimit && progress <= upperLimit) return 'orange';
  return 'green';
}
