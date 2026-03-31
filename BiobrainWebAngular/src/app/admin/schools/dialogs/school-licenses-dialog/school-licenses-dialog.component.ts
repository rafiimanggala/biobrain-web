import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import { SchoolLicensesDialogData } from './school-licenses-dialog-data';

@Component({
  selector: 'app-school-licenses-dialog',
  templateUrl: './school-licenses-dialog.component.html',
  styleUrls: ['./school-licenses-dialog.component.scss'],
})
export class SchoolLicensesDialog extends DialogComponent<SchoolLicensesDialogData, SchoolLicensesDialogData> {
  readonly licenseMinValue = 1;
  readonly licenseMaxValue = 2147483647;

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: SchoolLicensesDialogData,
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
