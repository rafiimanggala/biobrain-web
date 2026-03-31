import { Injectable } from '@angular/core';
import { AuthTokensService } from 'src/app/core/storage/auth-tokens.service';

import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { AuthApi } from '../api/auth.api';
import { CurrentUserService } from '../services/current-user.service';

@Injectable()
export class RefreshAuthorizationOperation {

  constructor(
    private readonly _api: AuthApi,
    private readonly _authTokensService: AuthTokensService,
    private readonly _currentUserService: CurrentUserService
  ) {
  }

  async perform(): Promise<SuccessOrFailedResult<{ accessToken: string }>> {
    try {
      let tokens = this._authTokensService.getTokens();
      if (!tokens || !tokens.refresh_token) return Result.failed();

      tokens = await this._api.client.refresh(tokens.refresh_token).toPromise();
      if (!tokens || !tokens.access_token) return Result.failed();

      // store user details and jwt token in local storage to keep user logged in between page refreshes
      this._authTokensService.setTokens(tokens);
      // let user = await this._currentUserService.user;
      // console.log(await this._currentUserService.user);
      // let count = 0;
      // while(user == await this._currentUserService.user){
      //   await new Promise( resolve => setTimeout(resolve, 500) );
      //   console.log(await this._currentUserService.user);
      //   count++;
      //   if(count > 6)
      //     break;
      // }
      
      return Result.success({ accessToken: tokens.access_token });
    } catch (e) {
      return Result.failed();
    }
  }
}
