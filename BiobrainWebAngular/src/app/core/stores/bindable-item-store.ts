import { Epic, Reducer, Store } from './store';
import { Observable, Subject } from 'rxjs';
import {
  filter,
  first,
  map,
  startWith,
  switchMap,
  withLatestFrom,
} from 'rxjs/operators';

import { OnDestroy } from '@angular/core';

export interface BindableItemState<TItem, TCriteria> {
  item: TItem | null;
  criteria: TCriteria | null;
  isLoading: boolean;
}

export interface BindAction<TCriteria> {
  criteria: TCriteria;
}

export type ReloadAction = Record<string, unknown>;

function startLoading<TItem, TCriteria>(): Reducer<BindableItemState<TItem, TCriteria>> {
  return state => ({ ...state, isLoading: true });
}

function setCriteria<TItem, TCriteria>(
  criteria: TCriteria
): Reducer<BindableItemState<TItem, TCriteria>> {
  return state => ({ ...state, criteria, isLoading: true });
}

function setItem<TItem, TCriteria>(
  item: TItem
): Reducer<BindableItemState<TItem, TCriteria>> {
  return state => ({ ...state, item, isLoading: false });
}

function bindListEpic<TItem, TCriteria>(
  getItem: (criteria: TCriteria) => Observable<TItem>
): Epic<BindableItemState<TItem, TCriteria>, BindAction<TCriteria>> {
  return state => actions =>
    actions.pipe(
      first(),
      withLatestFrom(state),
      filter(
        ([action, s]) => JSON.stringify(action.criteria) !== JSON.stringify(s.criteria)
      ),
      switchMap(([action, _]) =>
        getItem(action.criteria).pipe(
          map(item => setItem<TItem, TCriteria>(item)),
          startWith(setCriteria<TItem, TCriteria>(action.criteria))
        )
      )
    );
}

function reloadEpic<TItem, TCriteria>(
  getItem: (criteria: TCriteria) => Observable<TItem>
): Epic<BindableItemState<TItem, TCriteria>, ReloadAction> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      map(([_, s]) => s.criteria),
      filter(criteria => Boolean(criteria)),
      switchMap(criteria => {
        if (criteria) {
          return getItem(criteria).pipe(
            map(item => setItem<TItem, TCriteria>(item)),
            startWith(startLoading<TItem, TCriteria>())
          );
        }

        throw new Error('unreachable due to `filter(criteria => Boolean(criteria))`');
      })
    );
}

export abstract class BindableItemStore<TItem, TCriteria>
  extends Store<BindableItemState<TItem, TCriteria>>
  implements OnDestroy {
  public readonly item$: Observable<TItem | null> = this.state$.pipe(
    map(state => state.item)
  );

  protected readonly bindAction$ = new Subject<BindAction<TCriteria>>();
  protected readonly reloadAction$ = new Subject<ReloadAction>();

  protected constructor() {
    super({
      item: null,
      criteria: null,
      isLoading: false,
    });
    this.addEpic(this.bindAction$, bindListEpic(this.getItem.bind(this)));
    this.addEpic(this.reloadAction$, reloadEpic(this.getItem.bind(this)));
  }

  public bind(criteria: TCriteria): void {
    this.bindAction$.next({ criteria });
  }

  public updateCriteria(criteria: TCriteria): void {
    this.state$.next({criteria: criteria,isLoading: false, item: null});
  }

  public attachBinding(criteria$: Observable<TCriteria>): void {
    this.attach(
      'binding',
      criteria$.subscribe(criteria => this.bind(criteria))
    );
  }

  public reload(): void {
    this.reloadAction$.next({});
  }

  public attachReload(doReload$: Observable<any>): void {
    this.attach(
      'reload',
      doReload$.subscribe(_ => this.reload())
    );
  }

  protected abstract getItem(criteria: TCriteria): Observable<TItem>;
}
