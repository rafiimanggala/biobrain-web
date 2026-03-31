import { HttpClient, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StatusCodes } from 'http-status-codes';
import { Observable, Subscriber } from 'rxjs';

import { LogoutOperation } from '../operations/logout.operation';
import { RefreshAuthorizationOperation } from '../operations/refresh-authorization.operation';

// для удобства объявим тип который содержит инфу о запросе который вернул 401ую и наш "внутренний" подписчик
// "внутренний" - потому что мы будем обворачивать Observable который зашел к нам из инициатора
type CallerRequest = {
  subscriber: Subscriber<any>;
  failedRequest: HttpRequest<any>;
};

@Injectable({
  providedIn: 'root',
})
export class ErrorInterceptor implements HttpInterceptor {
  private _refreshInProgress = false;
  private _requests: CallerRequest[] = [];

  constructor(
    private readonly _http: HttpClient,
    private readonly _refreshAuthOperation: RefreshAuthorizationOperation,
    private readonly _logoutOperation: LogoutOperation,
  ) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // catch requests to application API only
    if (!request.url.includes('api/')) {
      return next.handle(request);
    }

    // оборачиваем Observable из вызывающего кода своим, внутренним Observable
    // далее вернем вызывающему коду Observable, который под нашим контролем здесь
    const observable = new Observable<HttpEvent<any>>(subscriber => {
      // как только вызывающий код сделает подписку мы попадаем сюда и подписываемся на наш HttpRequest
      // тобишь выполняем оригинальный запрос
      const originalRequestSubscription = next.handle(request).subscribe(
        response => {
          // оповещаем в инициатор (success) ответ от сервера
          subscriber.next(response);
        },
        (err: { status?: number }) => {
          if (err.status === StatusCodes.UNAUTHORIZED) {
            void this._handleUnauthorizedError(subscriber, request);
          } else {
            subscriber.error(err);
          }
        },
        () => {
          // комплит запроса, отрабатывает finally() инициатора
          subscriber.complete();
        },
      );

      return () => {
        // на случай если в вызывающем коде мы сделали отписку от запроса
        // если не сделать отписку и здесь, в dev tools браузера не увидим отмены запросов, т.к инициатор (например Controller) делает отписку от нашего враппера, а не от исходного запроса
        originalRequestSubscription.unsubscribe();
      };
    });

    // вернем вызывающему коду Observable, пусть сам решает когда делать подписку.
    return observable;
  }

  private async _handleUnauthorizedError(subscriber: Subscriber<any>, request: HttpRequest<any>): Promise<void> {
    this._requests.push({
      subscriber,
      failedRequest: request,
    });

    if (!this._refreshInProgress) {
      // делаем запрос на восстанавливение токена, и установим флаг, дабы следующие "401ые"
      // просто запоминались но не инициировали refresh
      this._refreshInProgress = true;
      const refreshResult = await this._refreshAuthOperation.perform();
      this._refreshInProgress = false;

      if (refreshResult.isFailed()) {
        // если по каким - то причинам запрос на рефреш не отработал, то делаем логаут
        await this._logoutOperation.perform();
      } else {
        // если токен рефрешнут успешно, повторим запросы которые накопились пока мы ждали ответ от рефреша
        this._repeatFailedRequests(`Bearer ${refreshResult.data.accessToken}`);
      }
    }
  }

  private _repeatFailedRequests(authHeader: any): void {
    this._requests.forEach(c => {
      // клонируем наш "старый" запрос, с добавлением новенького токена
      const requestWithNewToken = c.failedRequest.clone({
        headers: c.failedRequest.headers.set('Authorization', authHeader),
      });
      // и повторяем (помним с.subscriber - subscriber вызывающего кода)
      this._repeatRequest(requestWithNewToken, c.subscriber);
    });
    this._requests = [];
  }

  private _repeatRequest(requestWithNewToken: HttpRequest<any>, subscriber: Subscriber<any>): void {
    this._http.request(requestWithNewToken).subscribe(
      res => {
        subscriber.next(res);
      },
      (err: { status?: number }) => {
        if (err.status === StatusCodes.UNAUTHORIZED) {
          // if just refreshed, but for unknown reasons we got 401 again - logout user
          void this._logoutOperation.perform();
        }
        subscriber.error(err);
      },
      () => {
        subscriber.complete();
      },
    );
  }
}
