import { BindableListState, BindableListStoreBase } from './bindable-list-store';
import { Epic, Reducer } from './store';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, map, withLatestFrom } from 'rxjs/operators';

import { scrambledEquals } from 'src/app/share/helpers/array-scrumbled-equals';

export interface BindableFilterableListState<TItem> {
  searchText: string | undefined | null;
  unavailableItems: TItem[] | undefined | null;
}

export interface BindSearchTextAction {
  searchText: string;
}

export interface BindUnavailableItemsAction<TItem> {
  items: TItem[];
}

type State<TItem, TCriteria> = BindableListState<TItem, TCriteria, BindableFilterableListState<TItem>>;

function setUnavailableItems<TItem, TCriteria>(items: TItem[]): Reducer<State<TItem, TCriteria>> {
  return state => {
    const nextState = { ...state };
    nextState.nestedState = { ...nextState.nestedState, unavailableItems: items };
    return nextState;
  };
}

function setSearchText<TItem, TCriteria>(searchText: string): Reducer<State<TItem, TCriteria>> {
  return state => {
    const nextState = { ...state };
    nextState.nestedState = { ...nextState.nestedState, searchText };
    return nextState;
  };
}

function bindUnavailableItemsEpic<TItem, TCriteria>(): Epic<State<TItem, TCriteria>, BindUnavailableItemsAction<TItem>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      filter(([action, s]) => !scrambledEquals(action.items, s.nestedState.unavailableItems)),
      map(([action, _]) => {
        const { items } = action;
        return setUnavailableItems<TItem, TCriteria>(items);
      })
    );
}

function bindSearchTextEpic<TItem, TCriteria>(): Epic<State<TItem, TCriteria>, BindSearchTextAction> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      filter(([action, s]) => action.searchText !== s.nestedState.searchText),
      map(([action, _]) => {
        const { searchText } = action;
        return setSearchText<TItem, TCriteria>(searchText);
      })
    );
}

export abstract class BindableFilterableListStore<TItem, TCriteria> extends
  BindableListStoreBase<TItem, TCriteria, BindableFilterableListState<TItem>> {
  readonly availableItems$: Observable<TItem[]>;

  protected readonly bindSearchTextAction$ = new Subject<BindSearchTextAction>();
  protected readonly bindUnavailableItemsAction$ = new Subject<BindUnavailableItemsAction<TItem>>();

  private readonly _availableItemsSubject: ReplaySubject<TItem[]>;

  protected constructor() {
    super();

    this.addEpic(this.bindUnavailableItemsAction$, bindUnavailableItemsEpic());
    this.addEpic(this.bindSearchTextAction$, bindSearchTextEpic());

    const buffer = 1;
    this._availableItemsSubject = new ReplaySubject<TItem[]>(buffer);

    this.availableItems$ = this._availableItemsSubject.asObservable();

    this.subscriptions.set(
      Symbol.for('availableItems'),
      this.items$
        .pipe(
          withLatestFrom(this.state$),
          filter((items, _) => Boolean(items)),
          map(([items, s]) =>
            items
              .filter(item => {
                if (!s.nestedState.searchText) {
                  return true;
                }

                const text = this.getFilterableText(item).toLowerCase();
                return text.includes(s.nestedState.searchText.toLowerCase());
              })
              .filter(item => {
                const toSkip = s.nestedState.unavailableItems;
                if (toSkip === null || toSkip === undefined) {
                  return true;
                }

                if (toSkip.length === 0) {
                  return true;
                }

                return !toSkip.some(exclItem => this.itemEqual(item, exclItem));
              })
          )
        )
        .subscribe(x => this._availableItemsSubject.next(x))
    );
  }

  public bindUnavailableItems(items: TItem[]): void {
    this.bindUnavailableItemsAction$.next({ items });
  }

  public attachSearchText(searchText$: Observable<string | undefined>): void {
    this.attach(
      'text_filtering',
      searchText$.subscribe(searchText => this._bindSearchText(searchText ?? ''))
    );
  }

  public attachUnavailableItems(items$: Observable<TItem[]>): void {
    this.attach(
      'attach_unavailable_items',
      items$.subscribe(items => this.bindUnavailableItems(items))
    );
  }

  private _bindSearchText(searchText: string): void {
    this.bindSearchTextAction$.next({ searchText });
  }

  protected abstract getFilterableText(item: TItem): string;

  protected abstract itemEqual(x: TItem, z: TItem): boolean;
}
