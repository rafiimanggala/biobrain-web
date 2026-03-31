import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SchoolStatus } from 'src/app/api/enums/school-status.enum';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import { SchoolDialogData } from './school-dialog-data';

@Component({
  selector: 'app-school-dialog',
  templateUrl: './school-dialog.component.html',
  styleUrls: ['./school-dialog.component.scss'],
})
export class SchoolDialog extends DialogComponent<SchoolDialogData, SchoolDialogData> {
  readonly licenseMinValue = 1;
  readonly licenseMaxValue = 2147483647;

  statuses: { status: SchoolStatus, name: string }[] = [{ status: SchoolStatus.FreeTrial, name: this.strings.freeTrial }, { status: SchoolStatus.LiveCustomer, name: this.strings.liveCustomer }, { status: SchoolStatus.Archive, name: this.strings.archive }];

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: SchoolDialogData,
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

    this.dialogData.coursesIds = this.dialogData.settings.courses.filter(_ => _.isSelected).map(_ => _.courseId);

    this.close(DialogAction.save, this.data);
  }
}
