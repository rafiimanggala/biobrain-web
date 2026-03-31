import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../strings.service';

import { ConfirmationDialogData } from './confirmation.dialog-data';

@Component({
  selector: 'confirm-dialog',
  templateUrl: 'confirmation.dialog.html',
  styleUrls: ['confirmation.dialog.scss'],
})
export class ConfirmationDialog extends DialogComponent<ConfirmationDialogData, ConfirmationDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: ConfirmationDialogData,
  ) {
    super(dialogData);
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }

  onSubmit(): void {
    this.close(DialogAction.yes);
  }
}
