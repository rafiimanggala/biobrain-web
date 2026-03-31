import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { RenameClassDialogData } from './rename-class.dialog-data';

@Component({
  selector: 'rename-class-dialog',
  templateUrl: 'rename-class.dialog.html',
  styleUrls: ['rename-class.dialog.scss'],
})
export class RenameClassDialog extends DialogComponent<RenameClassDialogData, RenameClassDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: RenameClassDialogData,
  ) {
    super(dialogData);
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }

  onSubmit(): void {
    this.close(DialogAction.yes, this.dialogData);
  }
}
