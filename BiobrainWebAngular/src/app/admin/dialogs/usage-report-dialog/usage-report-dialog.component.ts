import { Component, Inject, ViewChild } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';
import { Moment } from 'moment';
import { MatDatepicker } from '@angular/material/datepicker';
import moment from 'moment';

@Component({
  selector: 'app-usage-report-dialog',
  templateUrl: './usage-report-dialog.component.html',
  styleUrls: ['./usage-report-dialog.component.scss'],
})
export class UsageReportDialogComponent extends DialogComponent<undefined, { from: Moment, to: Moment }> {
  @ViewChild('fromPicker') fromDatePicker: MatDatepicker<moment.Moment> | undefined;
  @ViewChild('toPicker') toDatePicker: MatDatepicker<moment.Moment> | undefined;
  fromDate = new FormControl(moment().startOf('day').subtract(21, 'day'));
  toDate = new FormControl(moment().endOf('day'));

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: undefined,
  ) {
    super(data);
  }

  fromDatepickerClick() {
    this.fromDatePicker?.open();
  }

  toDatepickerClick() {
    this.toDatePicker?.open();
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) return;

    const from = (<moment.Moment>this.fromDate.value).startOf('day');
    const to = (<moment.Moment>this.toDate.value).endOf('day');
    this.close(DialogAction.save, { from, to });
  }
}
