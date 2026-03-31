import { HttpClient, HttpEvent, HttpHeaders, HttpParams, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { HttpErrorHandler } from '../core/api/http-error-handler';
import { binToString } from '../share/helpers/bin-array-to-string';

import { ApiPath } from './api-path.service';
import { FileCommand } from './common/file-command';
import { RequestBase } from './common/requestBase';

@Injectable({
  providedIn: 'root'
})
export class Api {
  constructor(protected readonly http: HttpClient, protected readonly api: ApiPath) {
  }

  public send<T>(request: RequestBase<T>): Observable<T> {
    return this._sendRequest<unknown>(request).pipe(
      catchError(err => HttpErrorHandler.handleError<unknown>(err)),
      map(result => request.deserializeResult(result))
    );
  }

  public sendFile<T>(request: FileCommand<T>): Observable<T> {
    let file: File = request.file;
    let formData: FormData = new FormData();
    formData.set('file', file, file.name);
     return this.http.post<T>(request.getUrl(this.api), formData);
  }

  public observe<T>(request: RequestBase<T>): Observable<HttpEvent<T>> {
    return this._observeRequest<unknown>(request).pipe(
      catchError(err => HttpErrorHandler.handleError<unknown>(err)),
      map(event => event as HttpEvent<T>),
    );
  }

  public getJsonFileContent<T>(request: RequestBase<T>): Observable<T> {
    let params = new HttpParams();

    return this.http.get<T>(request.getUrl(this.api), { params }).pipe(
      tap((response: any) => {
        let dataType = response.type;
        let binaryData = [];
        binaryData.push(response);
        console.log(binToString(binaryData));
      })
    );
  }

  private _sendRequest<Result>(request: RequestBase<Result>): Observable<Result> {
    const url = request.getUrl(this.api);
    const method = request.getMethod();

    switch (method) {
      case 'get':
        return this._get(url, request);
      case 'post':
        return this._post(url, request);
    }
  }

  private _observeRequest<Result>(request: RequestBase<Result>): Observable<HttpEvent<Result>> {
    const url = request.getUrl(this.api);
    const method = request.getMethod();

    switch (method) {
      case 'get':
        return this._observeGet<Result>(url, request);
      case 'post':
        return this._observePost(url, request);
    }
  }

  private _observeGet<Result>(url: string, payload: any): Observable<HttpEvent<Result>> {
    let params = new HttpParams();

    for (const key in payload) {
      const value = payload[key] as unknown as object;
      if (value?.toString())
        params = params.set(key, value?.toString() ?? '');
    }

    return this.http.request(new HttpRequest("GET", url, { params, reportProgress: true }));
  }

  private _observePost<Result>(url: string, payload: RequestBase<Result>): Observable<HttpEvent<Result>> {
    let headers = payload.getHeader();
    if (!headers)
      headers = new HttpHeaders().set('Content-Type', 'application/json');

    const json = JSON.stringify(payload);

    return this.http.request(new HttpRequest("POST", url, { headers: headers, reportProgress: true, body: json }));
  }

  private _get<Result>(url: string, payload: any): Observable<Result> {
    let params = new HttpParams();

    for (const key in payload) {
      const value = payload[key] as unknown as object;
      if (value?.toString())
        params = params.set(key, value?.toString() ?? '');
    }

    return this.http.get<Result>(url, { params });
  }

  private _post<Result>(url: string, payload: RequestBase<Result>): Observable<Result> {
    let headers = payload.getHeader();
    if (!headers)
      headers = new HttpHeaders().set('Content-Type', 'application/json');

    const json = JSON.stringify(payload);

    return this.http.post<Result>(url, json, { headers });
  }
}

