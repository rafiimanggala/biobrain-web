import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { SelectDialogItem, SelectItemDialogData } from './select-item-dialog-data';



@Component({
  selector: 'app-select-item-dialog',
  templateUrl: './select-item-dialog.component.html',
  styleUrls: ['./select-item-dialog.component.scss']
})
export class SelectItemDialog extends DialogComponent<SelectItemDialogData, SelectDialogItem> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: SelectItemDialogData
  ) {
    super(data);
  }

  onClose(): void {
    this.close();
  }

  onSubmit(item: SelectDialogItem): void {
    this.close(DialogAction.save, item);
  }
}

