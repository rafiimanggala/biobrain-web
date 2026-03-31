export function assertUnreachableStatement(_value: never): never {
  throw new Error('Unreachable statement');
}
