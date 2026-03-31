import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-change-self-email-dialog',
  templateUrl: './change-self-email-dialog.component.html',
  styleUrls: ['./change-self-email-dialog.component.scss'],
})
export class ChangeSelfEmailDialogComponent extends DialogComponent<{ email: string }, { email: string; password: string }> {
  email = '';
  password = '';

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: { email: string },
  ) {
    super(data);

    this.email = data.email;
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) return;

    this.close(DialogAction.save, {
      email: this.email,
      password: this.password
    });
  }
}
