import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/api/api.service';
import { CreateQuestionAnswerInput, CreateQuestionCommand } from 'src/app/api/content/create-question.command';
import { UpdateQuestionCommand } from 'src/app/api/content/update-question.command';
import { QuestionType } from 'src/app/api/enums/question-type.enum';
import { SummernoteService } from 'src/app/admin/services/summernote/summernote.service';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import {
  CreateQuestionDialogAnswer,
  CreateQuestionDialogData,
  CreateQuestionDialogResult
} from './create-question-dialog-data';

interface QuestionTypeOption {
  code: number;
  label: string;
}

@Component({
  selector: 'app-create-question-dialog',
  templateUrl: './create-question-dialog.component.html',
  styleUrls: ['./create-question-dialog.component.scss'],
  providers: [SummernoteService]
})
export class CreateQuestionDialogComponent extends DialogComponent<CreateQuestionDialogData, CreateQuestionDialogResult> {
  public isSaving: boolean = false;
  public errorMessage: string | null = null;

  public readonly questionTypes: QuestionTypeOption[] = [
    { code: QuestionType.multipleChoice, label: 'Multiple Choice' },
    { code: QuestionType.freeText, label: 'Free Text' },
    { code: QuestionType.dropDown, label: 'Drop Down' },
    { code: QuestionType.completeSentense, label: 'Complete Sentence' },
    { code: QuestionType.trueFalse, label: 'True / False' },
    { code: QuestionType.orderList, label: 'Order List' },
    { code: QuestionType.swipe, label: 'Swipe' },
  ];

  constructor(
    public readonly strings: StringsService,
    public readonly summernoteService: SummernoteService,
    @Inject(MAT_DIALOG_DATA) public readonly data: CreateQuestionDialogData,
    private readonly _api: Api
  ) {
    super(data);

    if (!this.data.answers || this.data.answers.length === 0) {
      this.data.answers = [
        new CreateQuestionDialogAnswer('', true),
        new CreateQuestionDialogAnswer('', false),
      ];
    }
  }

  get isEditMode(): boolean {
    return !!this.data.questionId;
  }

  get dialogTitle(): string {
    return this.isEditMode ? 'Edit Question' : 'Create Question';
  }

  addAnswer(): void {
    this.data.answers = [...this.data.answers, new CreateQuestionDialogAnswer('', false)];
  }

  removeAnswer(index: number): void {
    if (this.data.answers.length <= 1) {
      return;
    }
    this.data.answers = this.data.answers.filter((_, i) => i !== index);
  }

  trackByIndex(index: number): number {
    return index;
  }

  get isAnswersValid(): boolean {
    if (!this.data.answers || this.data.answers.length === 0) {
      return false;
    }
    if (!this.data.answers.some(a => a.isCorrect)) {
      return false;
    }
    if (this.data.answers.some(a => !a.text || a.text.trim().length === 0)) {
      return false;
    }
    return true;
  }

  onClose(): void {
    this.close();
  }

  async onSubmit(form: NgForm): Promise<void> {
    if (!form.valid || this.isSaving || !this.isAnswersValid) {
      return;
    }

    this.isSaving = true;
    this.errorMessage = null;

    const answerPayload: CreateQuestionAnswerInput[] = this.data.answers.map(a => ({
      text: a.text,
      isCorrect: a.isCorrect,
      caseSensitive: false,
      score: a.isCorrect ? 1 : 0
    }));

    try {
      if (this.isEditMode) {
        await this._api.send(new UpdateQuestionCommand(
          this.data.questionId!,
          this.data.questionTypeCode,
          this.data.header,
          this.data.text,
          this.data.hint,
          this.data.feedBack,
          answerPayload
        )).toPromise();

        this.close(DialogAction.save, new CreateQuestionDialogResult(this.data.questionId!));
      } else {
        const result = await this._api.send(new CreateQuestionCommand(
          this.data.courseId,
          this.data.questionTypeCode,
          this.data.header,
          this.data.text,
          this.data.hint,
          this.data.feedBack,
          answerPayload,
          this.data.nodeId
        )).toPromise();

        this.close(DialogAction.save, new CreateQuestionDialogResult(result.questionId));
      }
    } catch (err: any) {
      this.errorMessage = err?.message ?? 'Failed to save question.';
      this.isSaving = false;
    }
  }
}
