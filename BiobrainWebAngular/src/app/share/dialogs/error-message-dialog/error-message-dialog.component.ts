import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../strings.service';

import { ErrorMessageDialogData } from './error-message-dialog.data';

@Component({
  selector: 'app-error-message-dialog',
  templateUrl: './error-message-dialog.component.html',
  styleUrls: ['./error-message-dialog.component.scss'],
})
export class ErrorMessageDialogComponent extends DialogComponent<ErrorMessageDialogData> {

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: ErrorMessageDialogData,
  ) {
    super(data);
  }

  public onClose(): void {
    this.close();
  }
}
