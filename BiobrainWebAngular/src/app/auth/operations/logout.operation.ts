import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthTokensService } from 'src/app/core/storage/auth-tokens.service';

import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { UnauthorizedException } from '../../core/exceptions/unauthorized.exception';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';

@Injectable()
export class LogoutOperation {

  constructor(
    private readonly _authTokensService: AuthTokensService,
    private readonly _router: Router,
    private readonly _appEvents: AppEventProvider,
  ) {
  }

  async perform(): Promise<SuccessOrFailedResult> {
    try {
      // remove user from local storage to log user out
      this._authTokensService.clearTokens();

      await this._router.navigate(['/login']);
      return Result.success();
    } catch (e) {
      if (e instanceof UnauthorizedException) return Result.failed();
      throw e;
    }
  }
}
