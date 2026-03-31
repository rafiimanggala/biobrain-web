import { Component, OnDestroy } from '@angular/core';

import { Subscription } from 'rxjs';

@Component({
  template: '',
})
export abstract class DisposableSubscriberComponent implements OnDestroy {
  protected readonly subscriptions: Subscription[] = [];

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  protected pushSubscribtions(...subscriptions: Subscription[]): void {
    this.subscriptions.push(...subscriptions);
  }
}
