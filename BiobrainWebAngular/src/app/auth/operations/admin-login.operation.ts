import { Injectable } from '@angular/core';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { InternalServerException } from 'src/app/core/exceptions/internal-server.exception';

import { BadRequestCommonException } from '../../core/exceptions/bad-request-common.exception';
import { AuthTokensService } from '../../core/storage/auth-tokens.service';
import { FailedOrSuccessResult, Result } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { AuthApi } from '../api/auth.api';
import { AdminLoginModel } from '../models/admin-login.model';


@Injectable()
export class AdminLoginOperation {
  constructor(
    private readonly _strings: StringsService,
    private readonly _api: AuthApi,
    private readonly _authTokensService: AuthTokensService,
    private readonly _appEvents: AppEventProvider,
  ) {
  }

  async perform(user: AdminLoginModel): Promise<FailedOrSuccessResult<string>> {
    try {
      const tokens = await this._api.client.adminLogin(user).toPromise();
      if (!tokens || !tokens.access_token) return Result.failed(this._strings.operationCanceled);

      // store user details and jwt token in local storage to keep user logged in between page refreshes
      this._authTokensService.setTokens(tokens);
      return Result.success();
    } catch (e:any) {
      if (e instanceof BadRequestCommonException) return Result.failed(e.message);
      if (e instanceof InternalServerException) return Result.failed(e.message);
      return Result.failed(e.error);
    }
  }
}
