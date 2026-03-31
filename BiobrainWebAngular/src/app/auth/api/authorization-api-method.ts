import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { HttpErrorHandler } from 'src/app/core/api/http-error-handler';
import { TokensModel } from '../../core/models/tokens.model';
import { LoginModel } from '../models/login.model';
import { RefreshTokenModel } from '../models/refresh-token.model';
import { AdminLoginModel } from '../models/admin-login.model';

export class AuthorizationApiMethod {
  constructor(
    protected httpClient: HttpClient,
    private readonly baseApiMethod: string,
  ) {
  }

  login(user: LoginModel): Observable<TokensModel> {
    return this.getTokens(user, 'password');
  }

  adminLogin(user: AdminLoginModel): Observable<TokensModel> {
    return this.getAdminTokens(user, 'password');
  }

  refresh(refreshToken: string): Observable<TokensModel> {
    var tokenModel = new RefreshTokenModel();
    tokenModel.refresh_token = refreshToken;
    return this.getTokens(tokenModel, 'refresh_token').pipe(map(tokens => {
      if (!tokens.refresh_token || tokens.refresh_token.length < 1) {
        tokens.refresh_token = refreshToken;
      }
      return tokens;
    }));
  }

  logout(): Observable<any> {
    return this.httpClient.post(this.baseApiMethod + '/logout', null).pipe(catchError(HttpErrorHandler.handleError));
  }

  private getAdminTokens(data: AdminLoginModel, grantType: string): Observable<TokensModel> {
    const headers = new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded');

    Object.assign(data, {grant_type: grantType, scope: 'offline_access'});

    const params = new URLSearchParams();
    for (const [key, value] of Object.entries(data)) {
      params.append(key, value);
    }

    return this.httpClient.post<TokensModel>(this.baseApiMethod, params.toString(), {headers: headers}).pipe(catchError(err => HttpErrorHandler.handleError<TokensModel>(err)));
  }

  private getTokens(data: RefreshTokenModel | LoginModel, grantType: string): Observable<TokensModel> {
    const headers = new HttpHeaders().set('Content-Type', 'application/x-www-form-urlencoded');

    Object.assign(data, {grant_type: grantType, scope: 'offline_access'});

    const params = new URLSearchParams();
    for (const [key, value] of Object.entries(data)) {
      params.append(key, value);
    }

    return this.httpClient.post<TokensModel>(this.baseApiMethod, params.toString(), {headers: headers}).pipe(catchError(err => HttpErrorHandler.handleError<TokensModel>(err)));
  }
}
