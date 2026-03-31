import { hasValue } from './has-value';

export function assertHasValue<T>(value: T, message?: string): asserts value is NonNullable<T> {
  if (!hasValue(value)) {
    throw new TypeError(hasValue(message) ? message : 'Value is null or undefined.');
  }
}
