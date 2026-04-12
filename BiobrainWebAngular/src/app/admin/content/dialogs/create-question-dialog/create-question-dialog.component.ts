import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/api/api.service';
import { CreateQuestionAnswerInput, CreateQuestionCommand } from 'src/app/api/content/create-question.command';
import { UpdateQuestionCommand } from 'src/app/api/content/update-question.command';
import { QuestionType } from 'src/app/api/enums/question-type.enum';
import { SummernoteService } from 'src/app/admin/services/summernote/summernote.service';
import { SnackBarService } from 'src/app/share/services/snack-bar.service';
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

interface BulkParsedQuestion {
  text: string;
  hint: string;
  feedBack: string;
  answers: { text: string; isCorrect: boolean }[];
  parseError?: string;
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
  public savedCount: number = 0;
  public lastSavedHeader: string | null = null;

  public bulkMode: boolean = false;
  public bulkText: string = '';
  public bulkParsed: BulkParsedQuestion[] = [];
  public bulkProgress: { current: number; total: number } | null = null;

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
    private readonly _api: Api,
    private readonly _snackBar: SnackBarService
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
    if (this.savedCount > 0 && !this.isEditMode) {
      this.close(DialogAction.save, new CreateQuestionDialogResult(''));
    } else {
      this.close();
    }
  }

  toggleBulkMode(): void {
    this.bulkMode = !this.bulkMode;
    this.bulkParsed = [];
    this.errorMessage = null;
  }

  parseBulk(): void {
    this.bulkParsed = [];
    this.errorMessage = null;
    const raw = this.normalizeText(this.bulkText || '').trim();
    if (!raw) {
      this.errorMessage = 'Paste some questions first.';
      return;
    }
    // Split on --- separators, blank lines before Q:, or numbered prefixes like "1." / "1)"
    const blocks = raw.split(/\n\s*---+\s*\n|\n{2,}(?=(?:Q\s*[:.)]|\d{1,3}\s*[.)]))/i);
    for (const block of blocks) {
      const parsed = this.parseSingleBlock(block.trim());
      if (parsed) this.bulkParsed.push(parsed);
    }
    if (this.bulkParsed.length === 0) {
      this.errorMessage = 'Could not parse any questions. Check the format.';
    }
  }

  private normalizeText(text: string): string {
    return text
      // Smart quotes → straight quotes
      .replace(/[\u2018\u2019\u02BC]/g, "'")
      .replace(/[\u201C\u201D]/g, '"')
      // Various dashes → hyphen
      .replace(/[\u2013\u2014]/g, '-')
      // Non-breaking spaces → regular
      .replace(/\u00A0/g, ' ')
      // Windows line endings
      .replace(/\r\n/g, '\n')
      .replace(/\r/g, '\n');
  }

  private parseSingleBlock(block: string): BulkParsedQuestion | null {
    if (!block) return null;
    const lines = block.split('\n').map(l => l.trim()).filter(l => l.length > 0);
    if (lines.length === 0) return null;

    let text = '';
    let hint = '';
    let feedBack = '';
    const answers: { text: string; isCorrect: boolean }[] = [];
    // Track if we're in the question text (multi-line) before first answer
    let readingQuestion = false;

    for (const line of lines) {
      // Question prefixes: "Q:", "Q.", "Q)", "1.", "1)", or first line without other markers
      const qMatch = line.match(/^(?:Q|question)\s*[:.)]\s*(.*)$/i);
      const numMatch = line.match(/^\d{1,3}\s*[.)]\s*(.+)$/);
      const hMatch = line.match(/^(?:H|Hint)\s*[:.)]\s*(.*)$/i);
      const fMatch = line.match(/^(?:F|Feedback|Fb|Explanation|Explain)\s*[:.)]\s*(.*)$/i);
      // Answers: A), A., a), (A), [A], *A), *A., * A)
      // Also support numbered: 1), 1. (when already in answers context, less common)
      const aMatch = line.match(/^\s*(\*)?\s*[\(\[]?\s*([A-Za-z])\s*[\)\].]\s*(.+)$/);

      if (hMatch) {
        hint = hMatch[1] || '';
        readingQuestion = false;
      } else if (fMatch) {
        feedBack = fMatch[1] || '';
        readingQuestion = false;
      } else if (qMatch) {
        text = text ? `${text}<br>${qMatch[1]}` : qMatch[1];
        readingQuestion = true;
      } else if (aMatch && (answers.length > 0 || text)) {
        // Only treat as answer if we already have question text
        answers.push({ text: aMatch[3].trim(), isCorrect: !!aMatch[1] });
        readingQuestion = false;
      } else if (numMatch && !text) {
        // First numbered line is the question
        text = numMatch[1];
        readingQuestion = true;
      } else if (readingQuestion && !aMatch) {
        // Multi-line question continuation
        text = text ? `${text}<br>${line}` : line;
      } else if (!text && answers.length === 0) {
        // First unmarked line = question text
        text = line;
        readingQuestion = true;
      }
    }

    if (!text || answers.length === 0) {
      return { text, hint, feedBack, answers, parseError: 'Missing question text or answers' };
    }
    if (!answers.some(a => a.isCorrect)) {
      return { text, hint, feedBack, answers, parseError: 'No answer marked correct (prefix with *)' };
    }
    return { text: `<p>${text}</p>`, hint, feedBack, answers };
  }

  removeBulkItem(index: number): void {
    this.bulkParsed = this.bulkParsed.filter((_, i) => i !== index);
  }

  async submitBulk(): Promise<void> {
    const valid = this.bulkParsed.filter(p => !p.parseError);
    if (valid.length === 0) {
      this.errorMessage = 'No valid questions to import.';
      return;
    }
    this.isSaving = true;
    this.errorMessage = null;
    this.bulkProgress = { current: 0, total: valid.length };
    let successCount = 0;
    try {
      for (const q of valid) {
        const answerPayload: CreateQuestionAnswerInput[] = q.answers.map(a => ({
          text: a.text,
          isCorrect: a.isCorrect,
          caseSensitive: false,
          score: a.isCorrect ? 1 : 0
        }));
        await this._api.send(new CreateQuestionCommand(
          this.data.courseId,
          this.data.questionTypeCode || 1,
          '',
          q.text,
          q.hint,
          q.feedBack,
          answerPayload,
          this.data.nodeId
        )).toPromise();
        successCount += 1;
        this.bulkProgress = { current: successCount, total: valid.length };
      }
      this.savedCount += successCount;
      this.lastSavedHeader = `${successCount} bulk questions`;
      this.bulkParsed = [];
      this.bulkText = '';
      this._snackBar.showMessage(`Imported ${successCount} question${successCount === 1 ? '' : 's'} successfully`);
    } catch (err: any) {
      this.errorMessage = `Imported ${successCount}/${valid.length}. Error: ${err?.message ?? 'Failed.'}`;
    } finally {
      this.isSaving = false;
      this.bulkProgress = null;
    }
  }

  async onSubmit(form: NgForm): Promise<void> {
    await this.doSubmit(form, false);
  }

  async onSaveAndAddAnother(form: NgForm): Promise<void> {
    await this.doSubmit(form, true);
  }

  private resetForm(form: NgForm): void {
    this.data.header = '';
    this.data.text = '';
    this.data.hint = '';
    this.data.feedBack = '';
    this.data.answers = [
      new CreateQuestionDialogAnswer('', true),
      new CreateQuestionDialogAnswer('', false),
    ];
    form.resetForm({
      questionType: this.data.questionTypeCode,
      header: '',
      text: '',
      hint: '',
      feedBack: ''
    });
  }

  private async doSubmit(form: NgForm, addAnother: boolean): Promise<void> {
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
        return;
      }

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

      this.savedCount += 1;
      this.lastSavedHeader = this.data.header || (this.data.text || '').replace(/<[^>]+>/g, '').slice(0, 50);

      if (addAnother) {
        this.resetForm(form);
        this.isSaving = false;
        return;
      }

      this.close(DialogAction.save, new CreateQuestionDialogResult(result.questionId));
    } catch (err: any) {
      this.errorMessage = err?.message ?? 'Failed to save question.';
      this.isSaving = false;
    }
  }
}
