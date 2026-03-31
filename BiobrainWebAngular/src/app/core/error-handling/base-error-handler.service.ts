import { HttpErrorResponse } from '@angular/common/http';
import { ErrorHandler, Injectable } from '@angular/core';
import { getExceptionMessage } from 'src/app/share/helpers/get-exception-message';

import { LogoutOperation } from '../../auth/operations/logout.operation';
import { AppEventProvider } from '../app/app-event-provider.service';
import { RequestValidationException } from '../exceptions/request-validation.exception';
import { Logger } from '../services/logger';

@Injectable()
export class BaseErrorHandler implements ErrorHandler {

  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _logoutOperation: LogoutOperation,
    private  readonly _logger: Logger
  ) {
  }

  handleError(error: Error | HttpErrorResponse | RequestValidationException): void {
    if (error instanceof HttpErrorResponse) {
      // Server or connection error happened
      if (!navigator.onLine) {
        // Handle offline error
      } else {
        if (error.status === 401) {
          void this._logoutOperation.perform();
        }
        if (error.status === 500) {
          this._appEvents.errorEmit(error.statusText);
        }
      }
    } else {
        this._appEvents.errorEmit(getExceptionMessage(error));
    }

    this._appEvents.errorEmit(error.message);
    // And log it to the console
    console.error(error);
    this._logger.logError(JSON.stringify(error));
  }
}
