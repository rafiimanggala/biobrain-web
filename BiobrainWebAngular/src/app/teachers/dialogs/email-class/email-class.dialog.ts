import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { EmailClassDialogData } from './email-class.dialog-data';

@Component({
  selector: 'email-class-dialog',
  templateUrl: 'email-class.dialog.html',
  styleUrls: ['email-class.dialog.scss'],
})
export class EmailClassDialog extends DialogComponent<EmailClassDialogData, EmailClassDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: EmailClassDialogData,
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
