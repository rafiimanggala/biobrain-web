import { Epic, Reducer, Store } from './store';
import { Observable, Subject, of } from 'rxjs';
import {
  filter,
  map,
  switchMap,
  withLatestFrom
} from 'rxjs/operators';

const noValueIdx = -1;

export enum SingleSelectStoreDefaultValueStrategy {
  Empty,
  SetFirst,
  SetSingleOnInit,
}

interface SetItemsAction<TItem> {
  items: TItem[];
}

interface SetValueAction<TValue> {
  value: TValue;
}

interface SetDefaultValueStrategyAction {
  strategy: SingleSelectStoreDefaultValueStrategy;
}

type SelectNextItemAction = unknown;

type SelectPrevItemAction = unknown;

interface SingleSelectState<TItem, TValue> {
  items: TItem[];
  dirtyValue: TValue | null;
  selectedIndex: number;
  selectedItem: TItem | null;
  selectedValue: TValue | null;
  defaultValueStrategy: SingleSelectStoreDefaultValueStrategy;
}

function canSelectNextItem<TItem, TValue>(
  state: SingleSelectState<TItem, TValue>
): boolean {
  return (
    state.items.length > 0 && state.selectedIndex < state.items.length
  );
}

function canSelectPrevItem<TItem, TValue>(
  state: SingleSelectState<TItem, TValue>
): boolean {
  return state.items.length > 0 && state.selectedIndex > 0;
}

function setItems<TItem, TValue>(
  items: TItem[]
): Reducer<SingleSelectState<TItem, TValue>> {
  return state => ({ ...state, items });
}

function setDirtyValue<TItem, TValue>(
  dirtyValue: TValue | null
): Reducer<SingleSelectState<TItem, TValue>> {
  return state => ({ ...state, dirtyValue });
}

function clearSelectedItem<TItem, TValue>(): Reducer<SingleSelectState<TItem, TValue>> {
  return state => ({
    ...state,
    selectedValue: null,
    selectedItem: null,
    selectedIndex: noValueIdx,
  });
}

function setSelectedItem<TItem, TValue>(
  value: TValue | null,
  index: number
): Reducer<SingleSelectState<TItem, TValue>> {
  return state => ({
    ...state,
    selectedValue: value,
    selectedItem: state.items[index],
    selectedIndex: index,
  });
}

function setDefaultValueStrategy<TItem, TValue>(
  defaultValueStrategy: SingleSelectStoreDefaultValueStrategy
): Reducer<SingleSelectState<TItem, TValue>> {
  return state => ({ ...state, defaultValueStrategy });
}

function setItemsEpic<TItem, TValue>(
  getIndexByValue: (items: TItem[], value: TValue | null) => number,
  getValueByIndex: (items: TItem[], index: number) => TValue | null
): Epic<SingleSelectState<TItem, TValue>, SetItemsAction<TItem>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      switchMap(([action, s]) => {
        const index = getIndexByValue(action.items, s.dirtyValue);

        if (index === noValueIdx) {
          if (shouldSetFirstItem(s.defaultValueStrategy, action.items)) {
            const newIndex = 0;
            const newValue = getValueByIndex(action.items, newIndex);

            return of(
              setItems<TItem, TValue>(action.items),
              setDirtyValue<TItem, TValue>(newValue),
              setSelectedItem<TItem, TValue>(newValue, newIndex)
            );
          }

          return of(
            setItems<TItem, TValue>(action.items),
            clearSelectedItem<TItem, TValue>()
          );
        }
        return of(
          setItems<TItem, TValue>(action.items),
          setSelectedItem<TItem, TValue>(s.dirtyValue, index)
        );
      })
    );
}

function setValueEpic<TItem, TValue>(
  getIndexByValue: (items: TItem[], value: TValue | null) => number,
  getValueByIndex: (items: TItem[], index: number) => TValue | null
): Epic<SingleSelectState<TItem, TValue>, SetValueAction<TValue>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      switchMap(([action, s]) => {
        const index = getIndexByValue(s.items, action.value);
        if (index === noValueIdx) {
          if (s.defaultValueStrategy === SingleSelectStoreDefaultValueStrategy.SetFirst && s.items.length > 0) {
            const newIndex = 0;
            const newValue = getValueByIndex(s.items, newIndex);
            return of(
              setDirtyValue<TItem, TValue>(newValue),
              setSelectedItem<TItem, TValue>(newValue, newIndex)
            );
          }

          return of(
            setDirtyValue<TItem, TValue>(action.value),
            clearSelectedItem<TItem, TValue>()
          );
        }
        return of(
          setDirtyValue<TItem, TValue>(action.value),
          setSelectedItem<TItem, TValue>(action.value, index)
        );
      })
    );
}

function setDefaultValueStrategyEpic<TItem, TValue>(
  getIndexByValue: (items: TItem[], value: TValue | null) => number,
  getValueByIndex: (items: TItem[], index: number) => TValue | null
): Epic<SingleSelectState<TItem, TValue>, SetDefaultValueStrategyAction> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      switchMap(([action, s]) => {
        const index = getIndexByValue(s.items, s.selectedValue);
        if (index !== noValueIdx) {
          return of(setDefaultValueStrategy<TItem, TValue>(action.strategy));
        }

        if (shouldSetFirstItem(action.strategy, s.items)) {
          const newIndex = 0;
          const newValue = getValueByIndex(s.items, newIndex);
          return of(
            setDefaultValueStrategy<TItem, TValue>(action.strategy),
            setDirtyValue<TItem, TValue>(newValue),
            setSelectedItem<TItem, TValue>(newValue, newIndex)
          );
        }

        return of(setDefaultValueStrategy<TItem, TValue>(action.strategy));
      })
    );
}

function shouldSetFirstItem<TItem>(
  defaultValueStrategy: SingleSelectStoreDefaultValueStrategy,
  items: TItem[]
): boolean {
  switch (defaultValueStrategy) {
    case SingleSelectStoreDefaultValueStrategy.SetFirst:
      return items.length > 0;

    case SingleSelectStoreDefaultValueStrategy.SetSingleOnInit:
    {
      const expectedLength = 1;
      return items.length === expectedLength;
    }

    default:
      return false;
  }
}

function selectNextItemEpic<TItem, TValue>(
  getValueByIndex: (items: TItem[], index: number) => TValue | null
): Epic<SingleSelectState<TItem, TValue>, SelectNextItemAction> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      filter(([_, s]) => canSelectNextItem(s)),
      switchMap(([_, s]) => {
        const increment = 1;
        const newIndex = s.selectedIndex + increment;
        const newValue = getValueByIndex(s.items, newIndex);
        return of(
          setDirtyValue<TItem, TValue>(newValue),
          setSelectedItem<TItem, TValue>(newValue, newIndex)
        );
      })
    );
}

function selectPrevItemEpic<TItem, TValue>(
  getValueByIndex: (items: TItem[], index: number) => TValue | null
): Epic<SingleSelectState<TItem, TValue>, SelectPrevItemAction> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      filter(([_, s]) => canSelectPrevItem(s)),
      switchMap(([_, s]) => {
        const decrement = 1;
        const newIndex = s.selectedIndex - decrement;
        const newValue = getValueByIndex(s.items, newIndex);
        return of(
          setDirtyValue<TItem, TValue>(newValue),
          setSelectedItem<TItem, TValue>(newValue, newIndex)
        );
      })
    );
}

export abstract class SingleSelectStore<TItem, TValue> extends Store<SingleSelectState<TItem, TValue>> {
  public value$: Observable<TValue | null> = this.state$.pipe(
    map(state => state.selectedValue)
  );
  public selectedItem$: Observable<TItem | null> = this.state$.pipe(
    map(state => state.selectedItem)
  );
  public selectedIndex$: Observable<number> = this.state$.pipe(
    map(state => state.selectedIndex)
  );
  public canSelectNext$: Observable<boolean> = this.state$.pipe(
    map(canSelectNextItem)
  );
  public canSelectPrev$: Observable<boolean> = this.state$.pipe(
    map(canSelectPrevItem)
  );

  private readonly _setItemsAction$ = new Subject<SetItemsAction<TItem>>();
  private readonly _setValueAction$ = new Subject<SetValueAction<TValue>>();
  private readonly _selectNextAction$ = new Subject<SelectNextItemAction>();
  private readonly _selectPrevAction$ = new Subject<SelectPrevItemAction>();
  private readonly _setDefaultValueStrategyAction$ = new Subject<SetDefaultValueStrategyAction>();

  constructor() {
    super({
      items: [],
      dirtyValue: null,
      selectedItem: null,
      selectedValue: null,
      selectedIndex: noValueIdx,
      defaultValueStrategy: SingleSelectStoreDefaultValueStrategy.Empty,
    });

    this.addEpic(
      this._setItemsAction$,
      setItemsEpic(
        this.getIndexByValue.bind(this),
        this.getValueByIndex.bind(this)
      )
    );
    this.addEpic(
      this._setValueAction$,
      setValueEpic(
        this.getIndexByValue.bind(this),
        this.getValueByIndex.bind(this)
      )
    );
    this.addEpic(
      this._selectNextAction$,
      selectNextItemEpic(this.getValueByIndex.bind(this))
    );
    this.addEpic(
      this._selectPrevAction$,
      selectPrevItemEpic(this.getValueByIndex.bind(this))
    );
    this.addEpic(
      this._setDefaultValueStrategyAction$,
      setDefaultValueStrategyEpic(
        this.getIndexByValue.bind(this),
        this.getValueByIndex.bind(this)
      )
    );
  }

  public setItems(items: TItem[]): void {
    this._setItemsAction$.next({ items });
  }

  public attachItems(items$: Observable<TItem[]>): void {
    this.attach(
      'items',
      items$.subscribe(items => this.setItems(items))
    );
  }

  public setValue(value: TValue): void {
    this._setValueAction$.next({ value });
  }

  public attachValue(value$: Observable<TValue>): void {
    this.attach(
      'value',
      value$.subscribe(value => this.setValue(value))
    );
  }

  public selectNext(): void {
    this._selectNextAction$.next({});
  }

  public selectPrev(): void {
    this._selectPrevAction$.next({});
  }

  public setDefaultValueStrategy(
    strategy: SingleSelectStoreDefaultValueStrategy
  ): void {
    this._setDefaultValueStrategyAction$.next({ strategy });
  }

  protected getIndexByValue(items: TItem[], value: TValue | null): number {
    if (items.length === 0) {
      return noValueIdx;
    }

    return items.findIndex(item => this.getItemValue(item) === value);
  }

  protected getValueByIndex(items: TItem[], index: number): TValue | null {
    if (items.length === 0) {
      return null;
    }

    const newValue = items[index];
    if (newValue === undefined) {
      return null;
    }

    return this.getItemValue(items[index]);
  }

  protected abstract getItemValue(item: TItem): TValue;
}
