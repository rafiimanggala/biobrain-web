import { AppEventProvider } from './app-event-provider.service';
import { BadRequestModelException } from '../../core/exceptions/bad-request-model.exception';
import { Injectable, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

@Injectable()
export class BaseComponent implements OnDestroy {
  public isLoading = false;

  protected subscriptions: Subscription[] = [];

  constructor(protected appEvents: AppEventProvider) {}
  
  ngOnDestroy(): void {
    this.subscriptions.forEach(x => x.unsubscribe());
  }

  startLoading(): void {
    this.isLoading = true;
  }

  endLoading(): void {
    this.isLoading = false;
  }

  protected error(errorMessage: string): void {
    this.appEvents.errorEmit(errorMessage);
    console.error(errorMessage);
  }

  protected handleError(error: any): void {
    if (error instanceof BadRequestModelException) {
      if (error.json) {
        let errorMessages = '';
        for (const [key, value] of Object.entries(error.json)) {
          errorMessages = `${errorMessages} ${key}: ${value}`;
        }
        this.error(errorMessages);
      }
      return;
    }
    this.error(error.message);
  }  

  protected pushSubscribtions(...subscriptions: Subscription[]): void {
    this.subscriptions.push(...subscriptions);
  }
}
