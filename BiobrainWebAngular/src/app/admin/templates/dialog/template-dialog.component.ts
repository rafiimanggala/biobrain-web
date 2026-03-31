import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';
import { TemplateDialogData } from './template-dialog-data';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';

@Component({
  selector: 'app-template-dialog',
  templateUrl: './template-dialog.component.html',
  styleUrls: ['./template-dialog.component.scss'],
})
export class TemplateDialog extends DialogComponent<TemplateDialogData, TemplateDialogData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: TemplateDialogData,
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

    this.data.courseIds = this.data.courses.filter(_ => _.isSelected).map(_ => _.courseId);

    this.close(DialogAction.save, this.data);
  }
}
