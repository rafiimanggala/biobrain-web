import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-use-access-code-dialog',
  templateUrl: './use-access-code-dialog.component.html',
  styleUrls: ['./use-access-code-dialog.component.scss'],
})
export class UseAccessCodeDialogComponent extends DialogComponent<undefined, UseAccessCodeDialogResult> {
  accessCode = '';

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: undefined,
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

    this.close(DialogAction.save, { accessCode: this.accessCode });
  }
}

export interface UseAccessCodeDialogResult {
  accessCode: string;
}
