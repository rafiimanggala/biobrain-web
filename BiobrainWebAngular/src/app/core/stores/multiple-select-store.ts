import { Epic, Reducer, Store } from './store';
import { Observable, Subject, of } from 'rxjs';
import {
  map,
  switchMap,
  withLatestFrom
} from 'rxjs/operators';

export enum MultipleSelectStoreDefaultValueStrategy {
  Empty,
  SetFirst,
}

interface MultipleSelectState<TItem, TValue> {
  items: TItem[];
  dirtyValues: TValue[];
  selectedIndexes: number[];
  selectedItems: TItem[];
  selectedValues: TValue[];
  defaultValueStrategy: MultipleSelectStoreDefaultValueStrategy;
}

const noValueIdx = -1;

interface SetItemsAction<TItem> {
  items: TItem[];
}

interface SetValueAction<TValue> {
  value: TValue;
}

interface SetValuesAction<TValue> {
  values: TValue[];
}

interface RemoveValueAction<TValue> {
  value: TValue;
}

interface SetDefaultValueStrategyAction {
  strategy: MultipleSelectStoreDefaultValueStrategy;
}

function setItems<TItem, TValue>(
  items: TItem[]
): Reducer<MultipleSelectState<TItem, TValue>> {
  return state => ({ ...state, items });
}

function setDirtyValues<TItem, TValue>(
  dirtyValues: TValue[]
): Reducer<MultipleSelectState<TItem, TValue>> {
  return state => ({ ...state, dirtyValues });
}

function clearSelectedItems<TItem, TValue>(): Reducer<MultipleSelectState<TItem, TValue>> {
  return state => ({
    ...state,
    selectedValues: [],
    selectedItems: [],
    selectedIndexes: [],
  });
}

function setSelectedItem<TItem, TValue>(
  value: TValue,
  index: number
): Reducer<MultipleSelectState<TItem, TValue>> {
  return state => ({
    ...state,
    selectedValues: state.selectedValues.concat(value),
    selectedItems: state.selectedItems.concat(state.items[index]),
    selectedIndexes: state.selectedIndexes.concat(index),
  });
}

function setSelectedItems<TItem, TValue>(
  values: TValue[],
  indexes: number[]
): Reducer<MultipleSelectState<TItem, TValue>> {
  return state => ({
    ...state,
    selectedValues: values,
    selectedItems: indexes.map(i => state.items[i]),
    selectedIndexes: indexes,
  });
}

function setDefaultValueStrategy<TItem, TValue>(
  defaultValueStrategy: MultipleSelectStoreDefaultValueStrategy
): Reducer<MultipleSelectState<TItem, TValue>> {
  return state => ({ ...state, defaultValueStrategy });
}

function setItemsEpic<TItem, TValue>(
  getIndexesByValues: (items: TItem[], value: TValue[]) => number[],
  getValueByIndex: (items: TItem[], index: number) => TValue | null
): Epic<MultipleSelectState<TItem, TValue>, SetItemsAction<TItem>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      switchMap(([action, s]) => {
        const indexes = getIndexesByValues(action.items, s.dirtyValues);

        const allDirtyValuesStillApplicable = s.dirtyValues.length > 0 && s.dirtyValues.length === indexes.length;
        if (allDirtyValuesStillApplicable) {
          return of(
            setItems<TItem, TValue>(action.items),
            setSelectedItems<TItem, TValue>(s.dirtyValues, indexes)
          );
        }

        if (indexes.length === 0 && shouldSetFirstItem(s.defaultValueStrategy, action.items)) {
          const newIndex = 0;
          const newValue = getValueByIndex(action.items, newIndex);

          if (newValue) {
            return of(
              setItems<TItem, TValue>(action.items),
              setDirtyValues<TItem, TValue>([newValue]),
              setSelectedItem<TItem, TValue>(newValue, newIndex)
            );
          }
        }

        return of(
          setItems<TItem, TValue>(action.items),
          clearSelectedItems<TItem, TValue>()
        );
      })
    );
}

function setValueEpic<TItem, TValue>(
  getIndexByValue: (items: TItem[], value: TValue) => number
): Epic<MultipleSelectState<TItem, TValue>, SetValueAction<TValue>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      switchMap(([action, s]) => {
        const index = getIndexByValue(s.items, action.value);
        if (index === noValueIdx) {
          return of(
            setDirtyValues<TItem, TValue>(s.dirtyValues)
          );
        }

        if (s.selectedValues.includes(action.value)) {
          return of(
            setDirtyValues<TItem, TValue>(s.dirtyValues)
          );
        }

        return of(
          setDirtyValues<TItem, TValue>(s.dirtyValues.concat(action.value)),
          setSelectedItem<TItem, TValue>(action.value, index)
        );
      })
    );
}

function setValuesEpic<TItem, TValue>(
  getIndexesByValues: (items: TItem[], value: TValue[]) => number[],
  getValueByIndex: (items: TItem[], value: number) => TValue | null
): Epic<MultipleSelectState<TItem, TValue>, SetValuesAction<TValue>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      switchMap(([action, s]) => {
        const indexes = getIndexesByValues(s.items, action.values);

        if (indexes.length !== action.values.length) {
          if (action.values.length === 0 && shouldSetFirstItem(s.defaultValueStrategy, s.items)) {
            const newIndex = 0;
            const newValue = getValueByIndex(s.items, newIndex);

            if (newValue) {
              return of(
                setDirtyValues<TItem, TValue>([newValue]),
                setSelectedItems<TItem, TValue>([newValue], [newIndex])
              );
            }
          }

          return of(
            setDirtyValues<TItem, TValue>(action.values)
          );
        }

        return of(
          setDirtyValues<TItem, TValue>(action.values),
          setSelectedItems<TItem, TValue>(action.values, indexes)
        );
      })
    );
}

function removeValueEpic<TItem, TValue>(
  getIndexByValue: (items: TItem[], value: TValue) => number
): Epic<MultipleSelectState<TItem, TValue>, RemoveValueAction<TValue>> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      switchMap(([action, s]) => {
        const index = getIndexByValue(s.items, action.value);
        if (index === noValueIdx) {
          return of(
            setDirtyValues<TItem, TValue>(s.dirtyValues)
          );
        }

        const values = s.selectedValues.filter(x => x !== action.value);
        const indexes = s.selectedIndexes.filter(x => x !== index);
        const dirtyValues = s.dirtyValues.filter(x => x !== action.value);

        return of(
          setDirtyValues<TItem, TValue>(dirtyValues),
          setSelectedItems<TItem, TValue>(values, indexes)
        );
      })
    );
}

function setDefaultValueStrategyEpic<TItem, TValue>(
  getValueByIndex: (items: TItem[], index: number) => TValue | null
): Epic<MultipleSelectState<TItem, TValue>, SetDefaultValueStrategyAction> {
  return state => actions =>
    actions.pipe(
      withLatestFrom(state),
      switchMap(([action, s]) => {
        if (s.selectedValues.length === 0 && shouldSetFirstItem(action.strategy, s.items)) {
          const newIndex = 0;
          const newValue = getValueByIndex(s.items, newIndex);

          if (newValue) {
            return of(
              setDefaultValueStrategy<TItem, TValue>(action.strategy),
              setDirtyValues<TItem, TValue>([newValue]),
              setSelectedItems<TItem, TValue>([newValue], [newIndex])
            );
          }
        }

        return of(setDefaultValueStrategy<TItem, TValue>(action.strategy));
      })
    );
}

function shouldSetFirstItem<TItem>(
  defaultValueStrategy: MultipleSelectStoreDefaultValueStrategy,
  items: TItem[]
): boolean {
  return defaultValueStrategy === MultipleSelectStoreDefaultValueStrategy.SetFirst && items.length > 0;
}

export abstract class MultipleSelectStore<TItem, TValue> extends Store<MultipleSelectState<TItem, TValue>> {
  public values$: Observable<TValue[]> = this.state$.pipe(
    map(state => state.selectedValues)
  );

  public selectedItems$: Observable<TItem[]> = this.state$.pipe(
    map(state => state.selectedItems)
  );

  public selectedIndexes$: Observable<number[]> = this.state$.pipe(
    map(state => state.selectedIndexes)
  );

  private readonly _setItemsAction$ = new Subject<SetItemsAction<TItem>>();
  private readonly _setValueAction$ = new Subject<SetValueAction<TValue>>();
  private readonly _setValuesAction$ = new Subject<SetValuesAction<TValue>>();
  private readonly _removeValueAction$ = new Subject<RemoveValueAction<TValue>>();
  private readonly _setDefaultValueStrategyAction$ = new Subject<SetDefaultValueStrategyAction>();

  constructor() {
    super({
      items: [],
      dirtyValues: [],
      selectedItems: [],
      selectedValues: [],
      selectedIndexes: [],
      defaultValueStrategy: MultipleSelectStoreDefaultValueStrategy.Empty,
    });

    this.addEpic(
      this._setItemsAction$,
      setItemsEpic(
        this.getIndexesByValues.bind(this),
        this.getValueByIndex.bind(this)
      )
    );

    this.addEpic(
      this._setValueAction$,
      setValueEpic(
        this.getIndexByValue.bind(this)
      )
    );

    this.addEpic(
      this._setValuesAction$,
      setValuesEpic(
        this.getIndexesByValues.bind(this),
        this.getValueByIndex.bind(this)
      )
    );

    this.addEpic(
      this._removeValueAction$,
      removeValueEpic(
        this.getIndexByValue.bind(this)
      )
    );

    this.addEpic(
      this._setDefaultValueStrategyAction$,
      setDefaultValueStrategyEpic(
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

  public setValues(values: TValue[]): void {
    this._setValuesAction$.next({ values });
  }

  public removeValue(value: TValue): void {
    this._removeValueAction$.next({ value });
  }

  public attachValue(value$: Observable<TValue>): void {
    this.attach(
      'set_value',
      value$.subscribe(value => this.setValue(value))
    );
  }

  public attachRemoveValue(value$: Observable<TValue>): void {
    this.attach(
      'remove_value',
      value$.subscribe(value => this.removeValue(value))
    );
  }

  public setDefaultValueStrategy(
    strategy: MultipleSelectStoreDefaultValueStrategy
  ): void {
    this._setDefaultValueStrategyAction$.next({ strategy });
  }

  protected getIndexByValue(items: TItem[], value: TValue): number {
    if (items.length === 0) {
      return noValueIdx;
    }

    return items.findIndex(item => this.getItemValue(item) === value);
  }

  protected getIndexesByValues(items: TItem[], values: TValue[]): number[] {
    if (items.length === 0) {
      return [];
    }

    return values.map(value => items.findIndex(item => this.getItemValue(item) === value));
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
