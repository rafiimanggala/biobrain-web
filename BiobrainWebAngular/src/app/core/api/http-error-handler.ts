import { HttpErrorResponse } from '@angular/common/http';
import { StatusCodes } from 'http-status-codes';
import { Observable } from 'rxjs';

import { BadRequestCommonException } from '../../core/exceptions/bad-request-common.exception';
import { BadRequestModelException } from '../../core/exceptions/bad-request-model.exception';
import { InternalServerException} from '../../core/exceptions/internal-server.exception';
import { NotFoundCommonException } from '../../core/exceptions/not-found-common.exception';
import { UnauthorizedException } from '../../core/exceptions/unauthorized.exception';
import { isValidationError, RequestValidationException } from '../exceptions/request-validation.exception';
import { UnprocessableEntityException } from '../exceptions/unprocessable-entity.exception';

export class HttpErrorHandler {
  public static handleError<T>(error: any): Observable<T> {
    if (error instanceof HttpErrorResponse && navigator.onLine) {
      if (error.status === StatusCodes.BAD_REQUEST && error.error) {
        const header = error.headers.get('X-Error-Type');
        if (header && header === 'InvalidModel') {
          const exception = new BadRequestModelException(error.error);
          exception.message = error.statusText;
          throw exception;
        }
      }

      if (error.status === StatusCodes.BAD_REQUEST && isValidationError(error.error)) {
        const exception = new RequestValidationException(error.message, error.error.errors);
        throw exception;
      }

      if (
        error.error &&
        error.status === StatusCodes.BAD_REQUEST &&
        error.error.error &&
        error.error.error_description
      ) {
        const exception = new BadRequestCommonException();
        exception.message = error.error.error_description;
        exception.name = error.error.error;
        console.error(error);
        throw exception;
      }

      if (error.status === StatusCodes.INTERNAL_SERVER_ERROR || error.status === StatusCodes.GATEWAY_TIMEOUT) {
        throw new InternalServerException(error.statusText);
      }

      // TODO: Temporary solution to don't miss information about command business logic checks on server side.
      // It's not internal server error.
      // The error should be processed in corresponding service / request class (will be implemented in next phase).
      if (error.status === StatusCodes.UNPROCESSABLE_ENTITY) {
        console.error(error);
        throw new UnprocessableEntityException(error.statusText, error.error.title);
      }

      if (error.status === StatusCodes.UNAUTHORIZED) {
        throw new UnauthorizedException(error.message);
      }

      if (error.status === StatusCodes.NOT_FOUND) {
        const exception = new NotFoundCommonException();
        exception.message = error.statusText;
        exception.name = error.status.toString();
        throw exception;
      }
    }

    throw error;
  }
}


