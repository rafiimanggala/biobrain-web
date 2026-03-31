import { Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig, MatDialogState } from '@angular/material/dialog';
import { Observable } from 'rxjs';

import { hasValue } from '../../share/helpers/has-value';

import { DialogAction } from './dialog-action';
import { DialogComponentType } from './dialog-component';
import { DialogOptions } from './dialog-options';
import { DialogResult } from './dialog-result';

@Injectable()
export class Dialog {
  constructor(private readonly _matDialog: MatDialog) {
  }

  public observe<TData, TResult>(component: DialogComponentType<TData, TResult>, data: TData, options?: Partial<DialogOptions>, afterClosedAction?: ()=>void): Observable<DialogResult<TResult> | DialogResult<undefined>> {
    return new Observable(subscriber => {
      const config = this._createOptions(data, options);
      const dialogRef = this._matDialog.open(component, config);

      const resultSubscription = dialogRef.componentInstance.result$.subscribe(
        result => subscriber.next(result),
        err => subscriber.error(err),
        () => subscriber.complete(),
      );

      const afterClosedSubscription = dialogRef.afterClosed().subscribe(
        _ => {
          subscriber.next(new DialogResult(DialogAction.close, undefined)); 
          if(afterClosedAction) afterClosedAction();
        },
        err => subscriber.error(err),
        () => subscriber.complete(),
      );

      return () => {
        resultSubscription.unsubscribe();
        afterClosedSubscription.unsubscribe();

        if (dialogRef.getState() === MatDialogState.OPEN) {
          dialogRef.close();
        }
      };
    });
  }

  public show<TData, TResult>(component: DialogComponentType<TData, TResult>, data: TData, options?: Partial<DialogOptions>, afterClosedAction?: ()=>void): Promise<DialogResult<TResult> | DialogResult<undefined>> {
    return this.observe(component, data, options, afterClosedAction).toPromise();
  }

  private _createOptions<TDialogData>(dialogData: TDialogData, options?: Partial<DialogOptions>): MatDialogConfig<TDialogData> {
    const config = new MatDialogConfig<TDialogData>();
    config.data = dialogData;

    config.autoFocus = false;
    config.restoreFocus = true;
    config.closeOnNavigation = true;
    config.disableClose = false;
    config.panelClass = 'dialog-panel';

    if (hasValue(options)) {
      return { ...config, ...options };
    }

    return config;
  }
}

