import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { hasValue } from 'src/app/share/helpers/has-value';

import { CurrentUserService } from '../../auth/services/current-user.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';

@Injectable({
  providedIn: 'root',
})
export class ActiveSchoolService {
  public readonly schoolIdChanges$: BehaviorSubject<string | null>;
  public readonly schoolNameChanges$: BehaviorSubject<string | null>;
  private readonly _activeSchoolIdKey = 'activeSchoolId';
  private readonly _activeSchoolNameKey = 'activeSchoolName';

  constructor(_currentUserService: CurrentUserService) {
    this.schoolIdChanges$ = new BehaviorSubject<string | null>(localStorage.getItem(this._activeSchoolIdKey));
    this.schoolNameChanges$ = new BehaviorSubject<string | null>(localStorage.getItem(this._activeSchoolNameKey));

    _currentUserService.userChanges$.subscribe(user => {
      if (!hasValue(user)) {
        this._cleanActiveSchoolId();
      }
    });
  }

  public get schoolId(): Promise<string | null> {
    return firstValueFrom(this.schoolIdChanges$);
  }

  public setSchoolId(value: string) {
    this.schoolIdChanges$.next(value);
    localStorage.setItem(this._activeSchoolIdKey, value);
  }

  public get schoolName(): Promise<string | null> {
    return firstValueFrom(this.schoolNameChanges$);
  }

  public setSchoolName(value: string) {
    this.schoolNameChanges$.next(value);
    localStorage.setItem(this._activeSchoolNameKey, value);
  }

  private _cleanActiveSchoolId(): void {
    localStorage.removeItem(this._activeSchoolIdKey);
    localStorage.removeItem(this._activeSchoolNameKey);
    this.schoolIdChanges$.next(null);
    this.schoolNameChanges$.next(null);
  }
}

