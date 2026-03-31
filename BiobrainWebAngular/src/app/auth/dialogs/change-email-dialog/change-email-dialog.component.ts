import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-change-password-dialog',
  templateUrl: './change-email-dialog.component.html',
  styleUrls: ['./change-email-dialog.component.scss'],
})
export class ChangeEmailDialogComponent extends DialogComponent<undefined, { email: string }> {
  email = '';

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
      email: this.email
    });
  }
}
