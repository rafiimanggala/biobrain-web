/* eslint-disable max-classes-per-file */

import { Epic, Reducer, Store } from './store';
import { Observable, Subject } from 'rxjs';
import {
  filter,
  map,
  startWith,
  switchMap,
  withLatestFrom,
} from 'rxjs/operators';

const noValueIdx = -1;

export interface BindableListState<TItem, TCriteria, TNestedState> {
  items: TItem[];
  isChangedAfterLoad: boolean;
  criteria: TCriteria | null;
  isLoading: boolean;
  nestedState: TNestedState;
}

export interface BindListAction<TCriteria> {
  criteria: TCriteria;
}

export interface BindSearchTextAction {
  searchText: string;
}

export interface BindUnavailableItemsAction<TItem> {
  items: TItem[];
}

export type ReloadAction = unknown;

export interface AddOrUpdateItemAction<TItem> {
  predicate: (item: TItem) => boolean;
  add?: () => TItem;
  update?: (item: TItem) => TItem;
}

function startLoading<TItem, TCriteria, TNested>(): Reducer<BindableListState<TItem, TCriteria, TNested>> {
  return state => ({ ...state, isLoading: true });
}

function setCriteria<TItem, TCriteria, TNested>(
  criteria: TCriteria
): Reducer<BindableListState<TItem, TCriteria, TNested>> {
  return state => ({ ...state, criteria, isLoading: true });
}

function setItems<TItem, TCriteria, TNested>(
  items: TItem[]
): Reducer<BindableListState<TItem, TCriteria, TNested>> {
  return state => ({
    ...state,
    items,
    isLoading: false,
    isChangedAfterLoad: false,
  });
}

function addItem<TItem, TCriteria, TNested>(
  item: TItem
): Reducer<BindableListState<TItem, TCriteria, TNested>> {
  return state => {
    const newItems = [...state.items];
    newItems.push(item);
    return { ...state, items: newItems, isChangedAfterLoad: true };
  };
}

function updateItem<TItem, TCriteria, TNested>(
  index: number,
  item: TItem
): Reducer<BindableListState<TItem, TCriteria, TNested>> {
  return state => {
    const newItems = [...state.items];
    newItems[index] = item;
    return { ...state, items: newItems, isChangedAfterLoad: true };
  };
}

function bindListEpic<TItem, TCriteria, TNested>(
  getItems: (criteria: TCriteria | null) => Observable<TItem[]>
): Epic<BindableListState<TItem, TCriteria, TNested>, BindListAction<TCriteria>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      filter(
        ([action, s]) => JSON.stringify(action.criteria) !== JSON.stringify(s.criteria)
      ),
      switchMap(([action, _]) =>
        getItems(action.criteria).pipe(
          map(items => setItems<TItem, TCriteria, TNested>(items)),
          startWith(setCriteria<TItem, TCriteria, TNested>(action.criteria))
        )
      )
    );
}

function reloadEpic<TItem, TCriteria, TNested>(
  getItems: (criteria: TCriteria | null) => Observable<TItem[]>
): Epic<BindableListState<TItem, TCriteria, TNested>, ReloadAction> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      filter(([_, s]) => Boolean(s.criteria)),
      switchMap(([_, s]) =>
        getItems(s.criteria).pipe(
          map(items => setItems<TItem, TCriteria, TNested>(items)),
          startWith(startLoading<TItem, TCriteria, TNested>())
        )
      )
    );
}

function addOrUpdateItemEpic<TItem, TCriteria, TNested>(
): Epic<BindableListState<TItem, TCriteria, TNested>, AddOrUpdateItemAction<TItem>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      map(([action, s]) => {
        const newItems = [...s.items];
        const index = newItems.findIndex(action.predicate);

        if (index === noValueIdx && action.add) {
          return addItem<TItem, TCriteria, TNested>(action.add());
        }

        if (index !== noValueIdx && action.update) {
          return updateItem<TItem, TCriteria, TNested>(
            index,
            action.update(newItems[index])
          );
        }

        return _ => _;
      })
    );
}

export abstract class BindableListStoreBase<TItem, TCriteria, TNestedState> extends
  Store<BindableListState<TItem, TCriteria, TNestedState>> {
  readonly items$: Observable<TItem[]> = this.state$.pipe(
    map(state => state.items)
  );

  readonly isLoading$: Observable<boolean> = this.state$.pipe(
    map(state => state.isLoading)
  );

  readonly isChangedAfterLoad$: Observable<boolean> = this.state$.pipe(
    map(state => state.isChangedAfterLoad)
  );

  protected readonly bindListAction$ = new Subject<BindListAction<TCriteria>>();
  protected readonly reloadAction$ = new Subject<ReloadAction>();
  protected readonly addOrUpdateItemAction$ = new Subject<AddOrUpdateItemAction<TItem>>();

  protected constructor() {
    super({
      items: [],
      criteria: null,
      isLoading: false,
      isChangedAfterLoad: false,
      nestedState: {} as TNestedState
    });

    this.addEpic(this.bindListAction$, bindListEpic(this.getItems.bind(this)));
    this.addEpic(this.reloadAction$, reloadEpic(this.getItems.bind(this)));
    this.addEpic(this.addOrUpdateItemAction$, addOrUpdateItemEpic());
  }

  public bind(criteria: TCriteria): void {
    this.bindListAction$.next({ criteria });
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

  protected abstract getItems(criteria: TCriteria | null): Observable<TItem[]>;
}

export abstract class BindableListStore<TItem, TCriteria> extends BindableListStoreBase<TItem, TCriteria, never> {
}
