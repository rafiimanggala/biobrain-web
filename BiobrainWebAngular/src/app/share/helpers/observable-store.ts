import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { mergeMap, switchMap, withLatestFrom } from 'rxjs/operators';

type ActionType<A> = new(...args: any[]) => A;
type Reducer<T, A> = (action: A, value: T) => T | Promise<T>;
type ObservableReducer<T, A> = (action: A, value: T) => Observable<T>;

export class ObservableStore<T> {
  private readonly _value$: BehaviorSubject<T>;
  private readonly _subscriptions = new Map<string, Subject<any>>();
  private readonly _reducers = new Map<string, Reducer<T, any>>();

  constructor(initialState: T) {
    this._value$ = new BehaviorSubject<T>(initialState);
  }

  public get value$(): Observable<T> {
    return this._value$;
  }

  public get currentValue(): T {
    return this._value$.getValue();
  }

  // eslint-disable-next-line @typescript-eslint/ban-types
  public dispatch<A extends object>(action: A): void {
    this._processSubscriptions(action, action.constructor.name);
    this._processReducers(action, action.constructor.name);
  }

  protected for<A>(actionType: ActionType<A>, reducer: Reducer<T, A>): void {
    this._reducers.set(actionType.name, reducer);
  }

  protected forEach<A>(actionType: ActionType<A>, reduce: ObservableReducer<T, A>): void {
    const action$ = new Subject<A>();

    action$
      .pipe(withLatestFrom(this._value$), mergeMap(([action, value]) => reduce(action, value)))
      .subscribe(value => this._value$.next(value));

    this._subscriptions.set(actionType.name, action$);
  }

  protected forLast<A>(actionType: ActionType<A>, reduce: ObservableReducer<T, A>): void {
    const action$ = new Subject<A>();

    action$
      .pipe(withLatestFrom(this._value$), switchMap(([action, value]) => reduce(action, value)))
      .subscribe(value => this._value$.next(value));

    this._subscriptions.set(actionType.name, action$);
  }

  private _processSubscriptions<TAction>(action: TAction, name: string): void {
    const subscription$ = this._subscriptions.get(name);
    if (!subscription$) {
      return;
    }

    subscription$.next(action);
  }

  private _processReducers<TAction>(action: TAction, name: string): void {
    const reducer = this._reducers.get(name);
    if (!reducer) {
      return;
    }

    const currentValue = this._value$.getValue();
    const newValue = reducer(action, currentValue);

    this._setValue(newValue);
  }

  private _setValue(newValue: Promise<T> | T): void {
    if (newValue instanceof Promise) {
      // eslint-disable-next-line promise/prefer-await-to-then
      void newValue.then(value => this._value$.next(value));
      return;
    }
    this._value$.next(newValue);
  }
}
