import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-change-self-password-dialog',
  templateUrl: './change-self-password-dialog.component.html',
  styleUrls: ['./change-self-password-dialog.component.scss'],
})
export class ChangeSelfPasswordDialogComponent extends DialogComponent<undefined, { oldPassword: string; newPassword: string }> {
  oldPassword = '';
  newPassword = '';

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: undefined,
  ) {
    super(data);
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) return;

    this.close(DialogAction.save, {
      oldPassword: this.oldPassword,
      newPassword: this.newPassword,
    });
  }
}
