import { BehaviorSubject, Observable } from 'rxjs';
import { shareReplay, switchMap } from 'rxjs/operators';

export class ObservableCache<T> {
  private readonly _cache = new Map<string, Observable<T>>();
  private readonly _updateActions = new Map<string, BehaviorSubject<number>>();

  public get(key: string, getObservable: () => Observable<T>): Observable<T> {
    if (!this._cache.has(key)) {
      const updateAction$ = new BehaviorSubject(Date.now());
      this._updateActions.set(key, updateAction$);

      const observable$ = updateAction$.pipe(switchMap(_ => getObservable()), shareReplay(1));
      this._cache.set(key, observable$);
    }

    return this._cache.get(key) as Observable<T>;
  }

  public reload(key?: string): void {
    if (!key) {
      for (const action$ of this._updateActions.values()) {
        action$.next(Date.now());
      }
      return;
    }

    this._updateActions.get(key)?.next(Date.now());
  }
}
