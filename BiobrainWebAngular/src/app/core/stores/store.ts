import { Observable, ReplaySubject, Subscription } from 'rxjs';
import { map, withLatestFrom } from 'rxjs/operators';

import { DestroybleStore } from './destroyble-store';

export type Reducer<TState> = (state: TState) => TState;

export type Epic<TState, TAction> = (
  state: Observable<TState>
) => (actions: Observable<TAction>) => Observable<Reducer<TState>>;

export function toEpic<TState, TAction>(
  reducerBuilder: (action: TAction) => Reducer<TState>
): Epic<TState, TAction> {
  return _state => actions =>
    actions.pipe(map(action => reducerBuilder(action)));
}

export abstract class Store<TState> extends DestroybleStore {
  protected readonly state$: ReplaySubject<TState>;

  protected constructor(initialState: TState) {
    super();

    const stateBufferSize = 1;
    this.state$ = new ReplaySubject<TState>(stateBufferSize);
    this.state$.next(initialState);
  }

  protected addEpic<TAction>(
    actions: Observable<TAction>,
    epic: Epic<TState, TAction>
  ): void {
    this.subscriptions.set(this.getUniqueSubscriptionKey(),
      epic(this.state$)(actions)
        .pipe(
          withLatestFrom(this.state$),
          map(([reducer, state]) => reducer(state))
        )
        .subscribe(newState => this.state$.next(newState))
    );
  }

  protected attach(subscriptionName: string, subscription: Subscription): void {
    const key = Symbol(subscriptionName);

    this.subscriptions.get(key)?.unsubscribe();
    this.subscriptions.set(key, subscription);
  }
}
