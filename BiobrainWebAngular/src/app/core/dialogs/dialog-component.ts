import { ComponentType } from '@angular/cdk/portal';
import { Component, Inject, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Subject, Subscription } from 'rxjs';

import { hasValue } from '../../share/helpers/has-value';

import { DialogAction } from './dialog-action';
import { DialogResult } from './dialog-result';

export type DialogComponentType<TData, TResult = undefined> = ComponentType<DialogComponent<TData, TResult>>;

@Component({
  template: ''
})
export abstract class DialogComponent<TData, TResult = undefined> implements OnDestroy {
  public result$ = new Subject<DialogResult<TResult> | DialogResult<undefined>>();
  protected readonly subscriptions: Subscription[] = [];

  protected constructor(@Inject(MAT_DIALOG_DATA) public readonly dialogData: TData) {
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  protected pushSubscribtions(...subscriptions: Subscription[]): void {
    this.subscriptions.push(...subscriptions);
  }

  protected sendResult(action: DialogAction, data?: TResult): void {
    const result = hasValue(data) ? new DialogResult<TResult>(action, data) : new DialogResult<undefined>(action, undefined);
    this.result$.next(result);
  }

  protected error(error: Error): void {
    this.result$.error(error);
  }

  protected close(action: DialogAction = DialogAction.close, data?: TResult): void {
    this.sendResult(action, data);
    this.result$.complete();
  }
}
