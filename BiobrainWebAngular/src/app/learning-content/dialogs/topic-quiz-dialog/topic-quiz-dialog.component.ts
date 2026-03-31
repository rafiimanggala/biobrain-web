import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../../share/strings.service';

import { TopicQuizDialogData } from './topic-quiz-dialog-data';
import { TopicQuizDialogResult } from './topic-quiz-dialog-result';

@Component({
  selector: 'app-topic-quiz-dialog',
  templateUrl: './topic-quiz-dialog.component.html',
  styleUrls: ['./topic-quiz-dialog.component.scss'],
})
export class TopicQuizDialogComponent extends DialogComponent<TopicQuizDialogData, TopicQuizDialogResult> {
  public readonly questionCountOptions = [20, 30, 40, 60];
  public selectedQuestionCount = 20;

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) data: TopicQuizDialogData,
  ) {
    super(data);
  }

  onClose(): void {
    this.close();
  }

  onStart(): void {
    const result = new TopicQuizDialogResult(this.selectedQuestionCount);
    this.close(DialogAction.save, result);
  }
}
