import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthTokensService } from 'src/app/core/storage/auth-tokens.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(
    private readonly _authTokensService: AuthTokensService,
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const tokens = this._authTokensService.getTokens();

    if (tokens && tokens.access_token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${tokens.access_token}`
        }
      });
    }

    return next.handle(request);
  }
}
