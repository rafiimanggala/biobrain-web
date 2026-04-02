import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';

import { SubsectionQuizDialogData } from './subsection-quiz-dialog-data';
import { SubsectionQuizDialogResult } from './subsection-quiz-dialog-result';

@Component({
  selector: 'app-subsection-quiz-dialog',
  templateUrl: './subsection-quiz-dialog.component.html',
  styleUrls: ['./subsection-quiz-dialog.component.scss'],
})
export class SubsectionQuizDialogComponent extends DialogComponent<SubsectionQuizDialogData, SubsectionQuizDialogResult> {
  public readonly questionCountOptions = [20, 30, 40, 60];
  public selectedQuestionCount = 20;

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: SubsectionQuizDialogData,
  ) {
    super(data);
  }

  onClose(): void {
    this.close();
  }

  onStart(): void {
    const result = new SubsectionQuizDialogResult(this.selectedQuestionCount);
    this.close(DialogAction.save, result);
  }
}
