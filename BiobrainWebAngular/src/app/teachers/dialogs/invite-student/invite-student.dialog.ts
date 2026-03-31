import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { InviteStudentDialogData } from './invite-student.dialog-data';

@Component({
  selector: 'invite-student-dialog',
  templateUrl: 'invite-student.dialog.html',
  styleUrls: ['invite-student.dialog.scss'],
})
export class InviteStudentDialog extends DialogComponent<InviteStudentDialogData, InviteStudentDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: InviteStudentDialogData,
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
