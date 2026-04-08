import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/api/api.service';
import { UpdateQuizSettingsCommand } from 'src/app/api/content/update-quiz-settings.command';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';
import { QuizSettingsDialogData, QuizSettingsDialogResult } from './quiz-settings-dialog-data';

const RANDOMIZE_KEY_PREFIX = 'biobrain.quiz.randomize.';

@Component({
  selector: 'app-quiz-settings-dialog',
  templateUrl: './quiz-settings-dialog.component.html',
  styleUrls: ['./quiz-settings-dialog.component.scss'],
})
export class QuizSettingsDialogComponent extends DialogComponent<QuizSettingsDialogData, QuizSettingsDialogResult> {
  isSaving: boolean = false;
  errorMessage: string | null = null;

  readonly presetLengths: number[] = [20, 30, 40, 60];
  useAllQuestions: boolean = false;

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: QuizSettingsDialogData,
    private readonly _api: Api
  ) {
    super(data);
    this.useAllQuestions = data.questionCount == null;
  }

  onPresetClick(value: number): void {
    this.data.questionCount = value;
    this.useAllQuestions = false;
  }

  onUseAllToggle(): void {
    if (this.useAllQuestions) {
      this.data.questionCount = null;
    } else {
      this.data.questionCount = this.data.totalQuestions || this.presetLengths[0];
    }
  }

  onClose(): void {
    this.close();
  }

  async onSubmit(form: NgForm): Promise<void> {
    if (!form.valid || this.isSaving) {
      return;
    }

    if (!this.useAllQuestions) {
      const count = Number(this.data.questionCount);
      if (!Number.isInteger(count) || count < 1) {
        this.errorMessage = 'Question count must be a positive integer.';
        return;
      }
      if (this.data.totalQuestions > 0 && count > this.data.totalQuestions) {
        this.errorMessage = `Question count cannot exceed available questions (${this.data.totalQuestions}).`;
        return;
      }
    }

    this.isSaving = true;
    this.errorMessage = null;

    try {
      await this._api.send(new UpdateQuizSettingsCommand(
        this.data.quizId,
        this.data.name?.trim() || null,
        this.useAllQuestions ? null : this.data.questionCount
      )).toPromise();

      try {
        localStorage.setItem(RANDOMIZE_KEY_PREFIX + this.data.quizId, this.data.randomizeOrder ? '1' : '0');
      } catch {
        // ignore storage errors
      }

      this.close(DialogAction.save, new QuizSettingsDialogResult(
        this.data.name,
        this.useAllQuestions ? null : this.data.questionCount,
        this.data.randomizeOrder
      ));
    } catch (err: any) {
      this.errorMessage = err?.message ?? 'Failed to save quiz settings.';
      this.isSaving = false;
    }
  }
}
