import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  SimpleChanges
} from '@angular/core';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { HintDialogData } from 'src/app/learning-content/dialogs/hint-dialog/hint-dialog-data';
import { HintDialogComponent } from 'src/app/learning-content/dialogs/hint-dialog/hint-dialog.component';
import { removeTags } from 'src/app/share/helpers/regex-helper';

import { Question } from '../../../api/content/content-data-models';
import { assertHasValue } from '../../../share/helpers/assert-has-value';
import { hasValue } from '../../../share/helpers/has-value';
import { RequestHandlingService } from '../../../share/services/request-parse.service';
import { StringsService } from '../../../share/strings.service';
import { DisplayedQuestionData } from '../question-content-shadow-dom-node/question-content-shadow-dom-node.component';

@Component({
  selector: 'app-question-content',
  templateUrl: './question-content.component.html',
  styleUrls: ['./question-content.component.scss'],
})
export class QuestionContentComponent implements OnChanges, OnInit, OnDestroy {
  @Input() question?: Question;
  @Input() questionIndex?: number;
  @Input() totalQuestions?: number;
  @Input() nodeHeader?: string;
  @Input() hintsEnabled = true;

  @Output() answerValueChange = new EventEmitter<string>();

  displayedQuestionDatum: DisplayedQuestionData | undefined;

  private readonly _subscriptions: Subscription[] = [];

  public get hintVisible(): boolean {
    return this.hintsEnabled && hasValue(this.displayedQuestionDatum) && hasValue(this.displayedQuestionDatum.question) && hasValue(this.displayedQuestionDatum.question.hint) && removeTags(this.displayedQuestionDatum.question.hint).length > 0;}

  constructor(
    private readonly _elementRef: ElementRef<HTMLElement>,
    private readonly _dialog: Dialog,
    private readonly _strings: StringsService,
    public readonly requestService: RequestHandlingService,
  ) {
  }

  ngOnInit(): void {
    this._subscriptions.push(
      this.requestService.answerValueSelected$.pipe(filter(hasValue)).subscribe(answerValue => this.answerValueChange.emit(answerValue)),
    );
  }

  ngOnDestroy(): void {
    this._subscriptions.forEach(_ => _.unsubscribe());
  }

  ngOnChanges(_: SimpleChanges): void {
    this._bindQuestionText();
  }

  private _bindQuestionText(): void {
    assertHasValue(this.question);
    assertHasValue(this.questionIndex);
    assertHasValue(this.totalQuestions);
    assertHasValue(this.nodeHeader);

    this.displayedQuestionDatum = {
      question: this.question,
      questionIndex: this.questionIndex,
      questionsCount: this.totalQuestions,
      nodeHeader: this.nodeHeader
    };
  }

  async openHintDialog() {
    if(!this.displayedQuestionDatum?.question.hint) return;

    const dialogData = new HintDialogData(this._strings.hint, this.displayedQuestionDatum.question.hint);
    await this._dialog.show(HintDialogComponent, dialogData, {panelClass: "hint-dialog-panel"});
  }
}

