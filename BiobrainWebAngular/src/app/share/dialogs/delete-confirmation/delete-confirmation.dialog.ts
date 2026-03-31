import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../strings.service';

import { DeleteConfirmationDialogData } from './delete-confirmation.dialog-data';

@Component({
  selector: 'confirm-dialog',
  templateUrl: 'delete-confirmation.dialog.html',
  styleUrls: ['delete-confirmation.dialog.scss'],
})
export class DeleteConfirmationDialog extends DialogComponent<DeleteConfirmationDialogData, DeleteConfirmationDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: DeleteConfirmationDialogData,
  ) {
    super(dialogData);
  }

  onClose(): void {
    this.dialogData.confirmed = false;
    this.close(DialogAction.cancel, this.dialogData);
  }

  onSubmit(): void {
    this.dialogData.confirmed = true;
    this.close(DialogAction.delete, this.dialogData);
  }
}
