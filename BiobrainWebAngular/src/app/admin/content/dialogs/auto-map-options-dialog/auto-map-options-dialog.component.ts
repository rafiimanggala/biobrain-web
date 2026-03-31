import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';
import { AutoMapOptionsDialogData, AutoMapOptionsDialogResult } from './auto-map-options-dialog-data';

@Component({
  selector: 'app-auto-map-options-dialog',
  templateUrl: './auto-map-options-dialog.component.html',
  styleUrls: ['./auto-map-options-dialog.component.scss']
})
export class AutoMapOptionsDialogComponent extends DialogComponent<AutoMapOptionsDialogData, AutoMapOptionsDialogResult> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: AutoMapOptionsDialogData,
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

    this.close(DialogAction.save, AutoMapOptionsDialogResult.updateResult(this.data));
  }

  onDelete(form: NgForm): void {
    this.close(DialogAction.delete, AutoMapOptionsDialogResult.stopAutoMapResult(this.data));
  }
}
