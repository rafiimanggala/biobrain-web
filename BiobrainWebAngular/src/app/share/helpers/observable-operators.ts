import { Observable, of, OperatorFunction } from 'rxjs';
import { catchError, filter, ignoreElements, map, share, shareReplay, startWith } from 'rxjs/operators';

export function toDictionary<TItem, TKey>(key: (i: TItem) => TKey, thisArg?: any): OperatorFunction<TItem[], Map<TKey, TItem>> {
  return map(items => items.reduce((m, i) => m.set(key(i), i), new Map<TKey, TItem>()), thisArg);
}

export interface LoadingProcess<T> {
  data$: Observable<T>;
  running$: Observable<boolean>;
  error$: Observable<any>;
}

export function matToLoadingProcess<T, R>(project: (value: T, index: number) => Observable<R>, thisArg?: any): OperatorFunction<T, LoadingProcess<R>> {
  return map((value, index) => loadingProcessOf(project(value, index)), thisArg);
}

export function loadingProcessOf<T>(source: Observable<T>): LoadingProcess<T> {
  const startEvent = 'EB5078F8-587C-49F4-97DE-845A2FA0B758';
  const source$ = source.pipe(startWith(startEvent), shareReplay(1));

  return {
    data$: source$.pipe(filter(_ => _ !== startEvent), map(_ => _ as T)),
    running$: source$.pipe(map(_ => _ === startEvent), catchError(_ => of(false))),
    error$: source$.pipe(ignoreElements(), catchError(err => of(err)))
  };
}
