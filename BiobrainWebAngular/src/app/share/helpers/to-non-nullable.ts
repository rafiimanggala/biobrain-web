import { hasValue } from './has-value';

export function toNonNullable<T>(value: T): NonNullable<T> {
  return toNonNullableWithError('Value is null or undefined.')(value);
}

export function toNonNullableWithError(err: string): <T>(value: T) => NonNullable<T> {
  return value => {
    if (hasValue(value)) {
      return value;
    }

    throw new TypeError(err);
  };
}

