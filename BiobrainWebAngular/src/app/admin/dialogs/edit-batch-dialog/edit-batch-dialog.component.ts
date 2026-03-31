import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';
import { BatchSettings } from './batch-settings';
import { EditBatchDialogData } from './edit-batch-dialog-data';

@Component({
  selector: 'app-edit-batch-dialog',
  templateUrl: './edit-batch-dialog.component.html',
  styleUrls: ['./edit-batch-dialog.component.scss'],
})
export class EditBatchDialog extends DialogComponent<EditBatchDialogData, BatchSettings> {
  batchSettings: BatchSettings;
  get selectedCoursesCount(): number { return this.dialogData.courses.filter(_ => _.isSelected).length; }

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: EditBatchDialogData,
  ) {
    super(data);
    this.batchSettings = new BatchSettings();
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid || this.selectedCoursesCount < 1) return;

    this.batchSettings.courseIds = this.dialogData.courses.filter(_ => _.isSelected).map(_ => _.courseId);
    this.close(DialogAction.save, this.batchSettings);
  }
}
