import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { hasValue } from 'src/app/share/helpers/has-value';

import { Api } from '../../api/api.service';
import { GetSchoolByIdQuery } from '../../api/schools/get-school-by-id.query';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';

@Injectable({
  providedIn: 'root',
})
export class ActiveSchoolService {
  public readonly schoolIdChanges$: BehaviorSubject<string | null>;
  public readonly schoolNameChanges$: BehaviorSubject<string | null>;
  public readonly aiDisabledChanges$: BehaviorSubject<boolean>;
  private readonly _activeSchoolIdKey = 'activeSchoolId';
  private readonly _activeSchoolNameKey = 'activeSchoolName';
  private readonly _aiDisabledKey = 'activeSchoolAiDisabled';
  private _lastFetchedSchoolId: string | null = null;

  constructor(
    private readonly _api: Api,
    _currentUserService: CurrentUserService,
  ) {
    this.schoolIdChanges$ = new BehaviorSubject<string | null>(localStorage.getItem(this._activeSchoolIdKey));
    this.schoolNameChanges$ = new BehaviorSubject<string | null>(localStorage.getItem(this._activeSchoolNameKey));
    this.aiDisabledChanges$ = new BehaviorSubject<boolean>(localStorage.getItem(this._aiDisabledKey) === 'true');

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
    this._fetchAiDisabledIfNeeded(value);
  }

  public get schoolName(): Promise<string | null> {
    return firstValueFrom(this.schoolNameChanges$);
  }

  public setSchoolName(value: string) {
    this.schoolNameChanges$.next(value);
    localStorage.setItem(this._activeSchoolNameKey, value);
  }

  public get aiDisabled(): boolean {
    return this.aiDisabledChanges$.value;
  }

  private _fetchAiDisabledIfNeeded(schoolId: string): void {
    if (!schoolId || schoolId === this._lastFetchedSchoolId) {
      return;
    }
    this._lastFetchedSchoolId = schoolId;

    const query = new GetSchoolByIdQuery(schoolId);
    this._api.send(query).pipe(take(1)).subscribe({
      next: result => {
        const disabled = result.aiDisabled ?? false;
        this.aiDisabledChanges$.next(disabled);
        localStorage.setItem(this._aiDisabledKey, String(disabled));
      },
      error: () => {
        // On error, default to AI enabled
        this.aiDisabledChanges$.next(false);
        localStorage.setItem(this._aiDisabledKey, 'false');
      },
    });
  }

  private _cleanActiveSchoolId(): void {
    localStorage.removeItem(this._activeSchoolIdKey);
    localStorage.removeItem(this._activeSchoolNameKey);
    localStorage.removeItem(this._aiDisabledKey);
    this.schoolIdChanges$.next(null);
    this.schoolNameChanges$.next(null);
    this.aiDisabledChanges$.next(false);
    this._lastFetchedSchoolId = null;
  }
}

