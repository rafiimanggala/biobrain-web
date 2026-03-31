import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import { TeacherDialogData } from './teacher-dialog-data';

@Component({
  selector: 'app-teacher-dialog',
  templateUrl: './teacher-dialog.component.html',
  styleUrls: ['./teacher-dialog.component.scss'],
})
export class TeacherDialog extends DialogComponent<TeacherDialogData, TeacherDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: TeacherDialogData,
  ) {
    super(data);
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) {
      return;
    }

    this.close(DialogAction.save, this.data);
  }
}
