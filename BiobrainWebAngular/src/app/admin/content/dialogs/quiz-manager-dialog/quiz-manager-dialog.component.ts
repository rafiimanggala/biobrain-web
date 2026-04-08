import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { KeyValue } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/api/api.service';
import { AttachQuestionsToNodeCommand } from 'src/app/api/content/attach-questions-to-node.command';
import { Question } from 'src/app/api/content/content-data-models';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';
import { CreateQuestionDialogComponent } from '../create-question-dialog/create-question-dialog.component';
import { CreateQuestionDialogData } from '../create-question-dialog/create-question-dialog-data';
import { QuestionType } from 'src/app/api/enums/question-type.enum';
import { QuizManagerDialogData, QuizManagerDialogResult } from './quiz-manager-dialog-data';

@Component({
  selector: 'app-quiz-manager-dialog',
  templateUrl: './quiz-manager-dialog.component.html',
  styleUrls: ['./quiz-manager-dialog.component.scss'],
})
export class QuizManagerDialogComponent extends DialogComponent<QuizManagerDialogData, QuizManagerDialogResult> {
  isSaving: boolean = false;
  errorMessage: string | null = null;
  dirty: boolean = false;
  searchText: string = '';

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public readonly data: QuizManagerDialogData,
    private readonly _api: Api,
    private readonly _dialog: Dialog
  ) {
    super(data);
  }

  get filteredQuestions(): Question[] {
    if (!this.searchText.trim()) return this.data.questions;
    const term = this.searchText.toLowerCase();
    return this.data.questions.filter(q =>
      (q.header || '').toLowerCase().includes(term) ||
      this.stripHtml(q.text || '').toLowerCase().includes(term)
    );
  }

  stripHtml(html: string): string {
    const tmp = document.createElement('div');
    tmp.innerHTML = html || '';
    return tmp.textContent || tmp.innerText || '';
  }

  onDrop(event: CdkDragDrop<Question[]>): void {
    if (event.previousIndex === event.currentIndex) return;
    moveItemInArray(this.data.questions, event.previousIndex, event.currentIndex);
    this.dirty = true;
  }

  async onDelete(question: Question): Promise<void> {
    const confirm = await firstValueFrom(
      this._dialog.observe(DeleteConfirmationDialog,
        new DeleteConfirmationDialogData(this.strings.question, question.header || 'this question')
      )
    );
    if (confirm.data?.confirmed !== true) return;

    this.data.questions = this.data.questions.filter(q => q.questionId !== question.questionId);
    this.dirty = true;
  }

  async onAddNew(): Promise<void> {
    const dialogData = new CreateQuestionDialogData(
      this.data.courseId,
      this.data.nodeId,
      null,
      QuestionType.multipleChoice,
      '',
      '',
      '',
      '',
      []
    );
    const result = await this._dialog.show(CreateQuestionDialogComponent, dialogData, { width: '720px' });
    if (result.action === DialogAction.save) {
      this.dirty = true;
      this.close(DialogAction.save, new QuizManagerDialogResult(true));
    }
  }

  async onSave(): Promise<void> {
    if (this.isSaving) return;
    this.isSaving = true;
    this.errorMessage = null;

    try {
      const questionIds: KeyValue<number, string>[] = this.data.questions.map((q, index) => ({
        key: index + 1,
        value: q.questionId
      }));

      await this._api.send(new AttachQuestionsToNodeCommand(
        this.data.nodeId,
        questionIds,
        true
      )).toPromise();

      this.close(DialogAction.save, new QuizManagerDialogResult(true));
    } catch (err: any) {
      this.errorMessage = err?.message ?? 'Failed to save quiz changes.';
      this.isSaving = false;
    }
  }

  onClose(): void {
    if (this.dirty) {
      this.close(DialogAction.save, new QuizManagerDialogResult(true));
    } else {
      this.close();
    }
  }
}
