import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';
import { SelectCourseDialogData, SelectCourseDialogResult } from './select-course-dialog-data';

@Component({
  selector: 'app-select-course-dialog',
  templateUrl: './select-course-dialog.component.html',
  styleUrls: ['./select-course-dialog.component.scss']
})
export class SelectCourseDialogComponent extends DialogComponent<SelectCourseDialogData, SelectCourseDialogResult> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: SelectCourseDialogData,
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
    if(!this.data.selectedCourseId) {this.close(); return;}

    this.close(DialogAction.next, new SelectCourseDialogResult(this.data.selectedCourseId));
  }
}
