import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { AddTeacherDialogData } from './add-teacher.dialog-data';

@Component({
  selector: 'add-teacher-dialog',
  templateUrl: 'add-teacher.dialog.html',
  styleUrls: ['add-teacher.dialog.scss'],
})
export class AddTeacherDialog extends DialogComponent<AddTeacherDialogData, AddTeacherDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: AddTeacherDialogData,
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
