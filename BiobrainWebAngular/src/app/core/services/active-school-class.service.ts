import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { CurrentUserService } from '../../auth/services/current-user.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';

@Injectable({
  providedIn: 'root',
})
export class ActiveSchoolClassService {
  public readonly schoolClassIdChanges$: BehaviorSubject<string | null>;
  private readonly _activeSchoolClassIdKey = 'activeSchoolClassId';

  constructor(currentUserService: CurrentUserService) {
    this.schoolClassIdChanges$ = new BehaviorSubject(localStorage.getItem(this._activeSchoolClassIdKey));

    currentUserService.userChanges$.subscribe(user => {
      if (!hasValue(user)) {
        this._cleanActiveSchoolClassId();
      }
    });
  }

  public get schoolClassId(): Promise<string | null> {
    return firstValueFrom(this.schoolClassIdChanges$);
  }

  public setActiveSchoolClassId(schoolClassId: string): void {
    localStorage.setItem(this._activeSchoolClassIdKey, schoolClassId);
    this.schoolClassIdChanges$.next(schoolClassId);
  }

  private _cleanActiveSchoolClassId(): void {
    localStorage.removeItem(this._activeSchoolClassIdKey);
    this.schoolClassIdChanges$.next(null);
  }
}


