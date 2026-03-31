import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { UserGuideNodeDialogData } from './user-guide-node-dialog-data';

@Component({
  selector: 'app-user-guide-node-dialog',
  templateUrl: './user-guide-node-dialog.component.html',
  styleUrls: ['./user-guide-node-dialog.component.scss'],
})
export class UserGuideNodeDialog extends DialogComponent<UserGuideNodeDialogData, UserGuideNodeDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: UserGuideNodeDialogData,
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

    this.close(DialogAction.save, this.data);
  }

  onDelete(): void{
    this.close(DialogAction.delete, this.data);
  }
}
