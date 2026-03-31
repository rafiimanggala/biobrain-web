import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';

import { SelectClassDialogData } from './select-class-dialog.data';

@Component({
  selector: 'app-select-class-dialog',
  templateUrl: './select-class-dialog.component.html',
  styleUrls: ['./select-class-dialog.component.scss'],
})
export class SelectClassDialog extends DialogComponent<SelectClassDialogData, string> {
  selectedClassId = '';

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: SelectClassDialogData,
  ) {
    super(data);
  }

  onCancel(): void {
    this.close(DialogAction.cancel);
  }

  onConfirm(): void {
    if (!this.selectedClassId) {
      return;
    }
    this.close(DialogAction.save, this.selectedClassId);
  }
}
