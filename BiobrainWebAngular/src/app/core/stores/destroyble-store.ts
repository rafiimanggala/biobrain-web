import { Injectable, OnDestroy } from '@angular/core';

import { Subscription } from 'rxjs';

@Injectable()
export abstract class DestroybleStore implements OnDestroy {
  protected readonly subscriptions: Map<symbol, Subscription>;
  private _lastGeneratedKey: number;

  constructor() {
    this.subscriptions = new Map<symbol, Subscription>();
    this._lastGeneratedKey = -1;
  }

  ngOnDestroy(): void {
    for (const subscription of this.subscriptions.values()) {
      subscription.unsubscribe();
    }
  }

  protected getUniqueSubscriptionKey(): symbol {
    this._lastGeneratedKey++;
    return Symbol(this._lastGeneratedKey);
  }
}
