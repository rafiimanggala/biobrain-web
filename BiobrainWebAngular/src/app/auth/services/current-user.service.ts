import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';
import { AuthTokensService } from 'src/app/core/storage/auth-tokens.service';

import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { toNonNullable } from '../../share/helpers/to-non-nullable';
import { StringsService } from '../../share/strings.service';
import { JwtPayloadModel } from '../models/jwt-payload.model';

import { CurrentUser } from './current-user';

@Injectable({
  providedIn: 'root',
})
export class CurrentUserService {
  public readonly userChanges$: Observable<CurrentUser | undefined>;

  constructor(
    private readonly _strings: StringsService,
    private readonly _authTokensService: AuthTokensService,
  ) {
    this.userChanges$ = this._authTokensService.payloadChanges$.pipe(
      map(payload => hasValue(payload) ? this._mapToUser(payload) : undefined),
      shareReplay(1),
    );
  }

  public get user(): Promise<CurrentUser | undefined> {
    return firstValueFrom(this.userChanges$);
  }

  private static _mapToRoles(claimValue: string[] | string | undefined): string[] {
    if (!hasValue(claimValue)) {
      return [];
    }

    if (Array.isArray(claimValue)) {
      return claimValue;
    }

    return [claimValue];
  }

  private _mapToUser(payload: JwtPayloadModel): CurrentUser {
    const userId = toNonNullable(payload.sub);
    const name = toNonNullable(payload.name);
    const roles = CurrentUserService._mapToRoles(payload.role);

    return new CurrentUser(
      userId,
      name,
      roles,
      payload.schoolId?.split(','),
      payload.adminSchoolId?.split(','),
      payload.schoolName,
      payload.given_name,
      payload.family_name,
      payload.subscription_status,
      payload.subscription_type
    );
  }
}
