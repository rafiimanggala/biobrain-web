import { Injectable } from '@angular/core';
import jwtDecode from 'jwt-decode';
import { BehaviorSubject, Observable } from 'rxjs';
import { distinctUntilChanged, map, shareReplay } from 'rxjs/operators';

import { compareJwtPayloadModel, JwtPayloadModel } from '../../auth/models/jwt-payload.model';
import { hasValue } from '../../share/helpers/has-value';
import { TokensModel } from '../models/tokens.model';

@Injectable({
  providedIn: 'root',
})
export class AuthTokensService {
  static readonly authTokensKey = 'biobrainUser';

  public readonly payloadChanges$: Observable<JwtPayloadModel | undefined>;
  private readonly _tokensChanges$: BehaviorSubject<TokensModel | undefined>;

  constructor() {
    this._tokensChanges$ = new BehaviorSubject(this.getTokens());

    this.payloadChanges$ = this._tokensChanges$.pipe(
      map(tokens => tokens?.access_token),
      map(accessToken => hasValue(accessToken) ? jwtDecode<JwtPayloadModel>(accessToken) : undefined),
      distinctUntilChanged(compareJwtPayloadModel),
      shareReplay(1),
    );
  }

  setTokens(tokens: TokensModel): void {
    localStorage.setItem(AuthTokensService.authTokensKey, JSON.stringify(tokens));
    this._tokensChanges$.next(tokens);
  }

  clearTokens(): void {
    localStorage.removeItem(AuthTokensService.authTokensKey);
    this._tokensChanges$.next(undefined);
  }

  getTokens(): TokensModel | undefined {
    const json = localStorage.getItem(AuthTokensService.authTokensKey);
    if (!json) {
      return undefined;
    }

    return JSON.parse(json) as TokensModel;
  }
}
