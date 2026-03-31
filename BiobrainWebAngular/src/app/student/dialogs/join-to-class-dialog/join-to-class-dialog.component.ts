import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-join-to-class-dialog',
  templateUrl: './join-to-class-dialog.component.html',
  styleUrls: ['./join-to-class-dialog.component.scss'],
})
export class JoinToClassDialogComponent extends DialogComponent<undefined, JoinToClassDialogResult> {
  classCode = '';

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

    this.close(DialogAction.save, { classCode: this.classCode });
  }
}

export interface JoinToClassDialogResult {
  classCode: string;
}
