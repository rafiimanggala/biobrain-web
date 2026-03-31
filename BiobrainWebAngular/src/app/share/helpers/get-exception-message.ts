import { RequestValidationException, validationExceptionToString } from "src/app/core/exceptions/request-validation.exception";

export function getExceptionMessage(error: any): string {
    if (error instanceof RequestValidationException) {
      return validationExceptionToString(error);
    }
    else{        
        return 'error' in error ? (error as { error: string }).error : error as string;
    }
}