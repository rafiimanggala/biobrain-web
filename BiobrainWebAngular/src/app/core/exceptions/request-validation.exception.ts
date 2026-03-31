import { hasValue } from '../../share/helpers/has-value';

export class RequestValidationException {
  constructor(public readonly message: string, public readonly errors: Record<string, string[]>) {
  }
}

export function validationExceptionToString(exception: RequestValidationException, devider: string = '\n'): string {
  let result = '';
  for (const key in exception.errors) {
    let array = exception.errors[key];
    result = result + array.join(devider);
  }
  if(!result)
    result = exception.message;
  return result;
}

export function isValidationError(
  error: { title?: string; errors?: Record<string, string[]> }
): error is { errors: Record<string, string[]> } {
  if (error.title !== 'One or more validation errors occurred.') {
    return false;
  }

  return hasValue(error.errors);
}
