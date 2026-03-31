import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../strings.service';
import { PickDateDialogData } from './pick-date-dialog-data';

@Component({
  selector: 'app-pick-date-dialog',
  templateUrl: './pick-date-dialog.component.html',
  styleUrls: ['./pick-date-dialog.component.scss'],
})
export class PickDateDialogComponent extends DialogComponent<PickDateDialogData, PickDateDialogData> {

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: PickDateDialogData,
  ) {
    super(data);
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) return;

    this.close(DialogAction.save, this.data);
  }
}
