import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { AuthConfig } from '../config';

import { AuthorizationApiMethod } from './authorization-api-method';

@Injectable({
  providedIn: 'root'
})
export class AuthApi {
  public readonly client: AuthorizationApiMethod;

  constructor(private readonly httpClient: HttpClient) {
    this.client = new AuthorizationApiMethod(httpClient, AuthConfig.AuthAPIPath);
  }
}
