import { Component, ElementRef, Input, OnDestroy, Renderer2, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Observable, ReplaySubject } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import Sortable from 'sortablejs';

import { Question } from '../../../api/content/content-data-models';
import { QuestionType } from '../../../api/enums/question-type.enum';
import { ThemeService } from '../../../core/app/theme.service';
import { assertHasValue } from '../../../share/helpers/assert-has-value';
import { assertUnreachableStatement } from '../../../share/helpers/assert-unreachable-statement';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { isNullOrWhitespace } from '../../../share/helpers/is-null-or-white-space';
import { rewriteImageUrls } from '../../../share/helpers/rewrite-image-urls';
import { RequestHandlingService } from '../../../share/services/request-parse.service';
import { ContentStyles } from '../../../share/values/content-styles';
import { PluggableScriptComponent } from '../learning-material-shadow-dom-node/pluggable-script.component';

@Component({
  selector: 'app-question-content-shadow-dom-node',
  templateUrl: './question-content-shadow-dom-node.component.html',
  styleUrls: ['./question-content-shadow-dom-node.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom
})
export class QuestionContentShadowDomNodeComponent extends PluggableScriptComponent implements OnDestroy {
  questionData$ = new ReplaySubject<DisplayedQuestionData>(1);
  formattedHtml$: Observable<SafeHtml>;

  private readonly _handler: EventListener;
  private readonly _shadowRoot: ShadowRoot;

  private readonly _getRequestScriptId = 'sendRequestScript';
  private readonly _preventClickScriptId = 'preventClickScript';
  private readonly _preventTapScriptId = 'preventTapScript';

  private readonly _buttonsQuestionsRegEx = /(<button .*?)(type=["']submit["'].*?)(value=["'](.*?)["'])(.*?>.*?\n*?.*?<\/button>)/g;
  private readonly _buttonsQuestionsExpression = "$1 type=\"button\" onclick=\"sendRequest(event, 'answer.response1=$4')\"$5";

  private readonly _columnHeadersRegEx = /<center>(<button .*?)(type=["']submit["'].*?)(value=["'](.*?)["'])(.*?>.*?\n*?.*?<\/button>)<\/center>/g;
  private readonly _columnHeadersExpression = "<div class=\"columnHeaders\">$1$2$3$5</div>";


  // private readonly _answerButtonRegEx = /(<button .*?class=["']answerButton["'].*?)(.*?><img .*?src=["'])(.*?\/AnswerButton.png)(["'].*?><\/button>)/g;
  private readonly _answerButtonImageRegEx = /<img[^>]*?src=["'][^>]*?\/AnswerButton.png["'][^>]*?>/g;
  private readonly _answerButtonRegEx = /(<button .*?class=["']answerButton["'].*?)(.*?>)/g;
  private readonly _dropDownAnswerReplaceExpression = '$1 type="button" onClick="sendRequest(event)" $2';
  private readonly _swipeAnswerReplaceExpression = '$1 type="button" onClick="sendRequest(event)" $2';
  private readonly _orderListAnswerReplaceExpression = '$1 type="button" onClick="sendRequest(event)" $2';
  private readonly _freeTextAnswerReplaceExpression = '$1 type="button" onClick="sendRequest(event)" $2';


  private readonly _linksRegEx = /(<a .*?)(href=["'](.*?)["'])(.*?>.*?\n*?.*?<\/a>)/g;
  private readonly _linksReplaceExpression = "$1onclick=\"sendRequest(event, '$3')\"$4";

  constructor(
    private readonly _element: ElementRef<HTMLDivElement>,
    private readonly _requestHandlingService: RequestHandlingService,
    private readonly _sanitizer: DomSanitizer,
    private readonly _themeService: ThemeService,
    renderer: Renderer2
  ) {
    super(renderer);

    assertHasValue(this._element.nativeElement.shadowRoot);
    this._shadowRoot = this._element.nativeElement.shadowRoot;

    this.formattedHtml$ = this.questionData$.pipe(
      switchMap(x => this._buildPlainQuestionContent(x)),
      map(x => this._sanitizer.bypassSecurityTrustHtml(x))
    );

    const handler: (ev: CustomEvent<{ev: any, request: string}>) => void = async (ev: CustomEvent<{ev: any, request: string}>) => {
      const datum = await firstValueFrom(this.questionData$);
      const value = this._getAnswerValue(Number(datum.question.questionTypeCode), ev);
      ev.detail.request = value;
      this._requestHandlingService.handle(ev.detail);
    };
    this._handler = handler as EventListener;

    this._shadowRoot.addEventListener('request', this._handler, false);
    this._includeRequestSenderScript();
  }

  @Input() set displayedQuestionDatum(value: DisplayedQuestionData | null | undefined) {
    assertHasValue(value);
    this.questionData$.next(value);
  }

  ngOnDestroy(): void {
    this._shadowRoot.removeEventListener('request', this._handler, false);
  }

  async initSortableIfNeed(): Promise<void> {
    const datum = await firstValueFrom(this.questionData$);

    if (Number(datum.question.questionTypeCode) === QuestionType.swipe) {
      this._initSwipeSortable();
    } else if (Number(datum.question.questionTypeCode) === QuestionType.orderList) {
      this._initOrderListSortable();
    }
  }

  protected getRootNode(): DocumentFragment {
    return this._shadowRoot;
  }

  private async _buildPlainQuestionContent(datum: DisplayedQuestionData): Promise<string> {
    const questionNumber = `<span class='questionNumber'>Q${datum.questionIndex}/${datum.questionsCount}</span>`;
    const levelName = datum.isCustomQuiz ? '' : `<span class='levelName'>${datum.nodeHeader.toUpperCase()}</span>`;
    const header = `<html><h2 class='questionText'>${questionNumber}${levelName}</h2>`;

    let questionText = datum.question.text.replace('<html>', header);
    questionText = await this._replaceLinks(questionText, Number(datum.question.questionTypeCode));

    const colors = await firstValueFrom(this._themeService.colors$);
    questionText = `<html>${ContentStyles.getQuestionStyles(colors)}<section><article class="questionText">${questionText}</article></section></html>`;

    return rewriteImageUrls(questionText);
  }

  private async _replaceLinks(text: string, questionType: QuestionType): Promise<string> {
    if (isNullOrWhitespace(text)) {
      return '';
    }

    // Replace href to onclick sendRequest
    text = text.replace(this._linksRegEx, this._linksReplaceExpression);

    const colors = await firstValueFrom(this._themeService.colors$);
    const svg = this._getAnswerButtonSvg(colors.primary);
    text = text.replace(this._answerButtonImageRegEx, `${svg}`);

    switch (questionType) {
      case QuestionType.multipleChoice:
      case QuestionType.trueFalse:
        return text.replace(this._buttonsQuestionsRegEx, this._buttonsQuestionsExpression);

      case QuestionType.freeText:
        return text.replace(this._answerButtonRegEx, this._freeTextAnswerReplaceExpression);

      case QuestionType.dropDown:
      case QuestionType.completeSentense:
        return text.replace(this._answerButtonRegEx, this._dropDownAnswerReplaceExpression);

      case QuestionType.orderList:
        return text.replace(this._answerButtonRegEx, this._orderListAnswerReplaceExpression);

      case QuestionType.swipe:
        text = text.replace(this._answerButtonRegEx, this._swipeAnswerReplaceExpression);
        text = text.replace(this._columnHeadersRegEx, this._columnHeadersExpression);
        return text;

      default:
        assertUnreachableStatement(questionType);
    }
  }

  private _includeRequestSenderScript(): void {
    this.includeScript(
      this._getRequestScriptId,
      'function sendRequest(ev, request) {' +
      'const event = new CustomEvent(\'request\', ' +
      '{bubbles: true, detail: {ev,request}}); ' +
      'ev.target.dispatchEvent(event);' +
      '}'
    );
    this.includeScript(this._preventClickScriptId, "document.querySelector('app-question-content-shadow-dom-node').addEventListener('contextmenu', e => { e.preventDefault(); });");
    this.includeScript(this._preventTapScriptId, "document.body.style.webkitTouchCallout='none';");
  }

  private _getAnswerValue(questionType: QuestionType, event: CustomEvent<{ev: any, request: string}>): string {
    switch (questionType) {
      case QuestionType.multipleChoice:
      case QuestionType.trueFalse:
        assertHasValue(event.detail);
        return event.detail.request;

      case QuestionType.dropDown:
      case QuestionType.completeSentense:
        return this._getDropDownValue();

      case QuestionType.freeText:
        return this._getFreeTextValue();

      case QuestionType.orderList:
        return this._getOrderListValue();

      case QuestionType.swipe:
        return this._getSwipeValue();

      default:
        assertUnreachableStatement(questionType);
    }
  }

  private _getDropDownValue(): string {
    const selectsBoxes = this._shadowRoot.querySelectorAll('.selectBox');
    const selects = [];

    if (selectsBoxes.length === 0) {
      return 'answer.';
    }

    for (let i = 0; i < selectsBoxes.length; i++) {
      selects.push(selectsBoxes[i].getElementsByTagName('select')[0]);
    }

    let result = 'answer.';
    selects.forEach(x => {
      result = result + x.name + '="' + x.value + '"&';
    });

    return result.substring(0, result.length - 1);
  }

  private _getSwipeValue(): string {
    const shadowRoot = this._shadowRoot;
    function getResultForSwipeResponse(element: string, response: string): string {
      const elements = shadowRoot.getElementById(element)?.getElementsByClassName('task');
      assertHasValue(elements);

      let result = response + '=[';
      for (let i = 0; i < elements.length; i++) {
        const ans = elements[i].getElementsByTagName('p')[0].innerText;
        result = result + '"' + ans + '"';
        if (i !== elements.length - 1) {
          result = result + ',';
        }
      }

      return result + ']';
    }

    let result = 'answer.';
    result = result + getResultForSwipeResponse('responce1', 'response1');
    result = result + '&';
    result = result + getResultForSwipeResponse('responce2', 'response2');
    return result;
  }

  private _getOrderListValue(): string {
    const elements = this._shadowRoot.getElementById('order')?.getElementsByClassName('task');
    assertHasValue(elements);

    let result = 'answer.';
    for (let i = 0; i < elements.length; i++) {
      result = `${result}response${i + 1}=${elements[i].getElementsByTagName('p')[0].innerText}`;
      if (i !== elements.length - 1) {
        result = result + '&';
      }
    }
    return result;
  }

  private _getFreeTextValue(): string {
    const element = this._shadowRoot.querySelector('textarea');
    assertHasValue(element);
    return 'answer.response1="' + element.value + '"';
  }

  private _initSwipeSortable(): void {
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const a = new Sortable(this._shadowRoot.getElementById('center') ?? new HTMLElement(), { group: 'name' });
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const b = new Sortable(this._shadowRoot.getElementById('responce1') ?? new HTMLElement(), { group: 'name' });
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const c = new Sortable(this._shadowRoot.getElementById('responce2') ?? new HTMLElement(), { group: 'name' });
  }

  private _initOrderListSortable(): void {
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    const a = new Sortable(this._shadowRoot.getElementById('order') ?? new HTMLElement(), {
      group: 'shared',
      animation: 150,
    });
  }

  private _getAnswerButtonSvg(color: string): string {
    return `
<svg xmlns="http://www.w3.org/2000/svg" version="1.0" width="48px" height="48px" viewBox="0 0 160.000000 160.000000" preserveAspectRatio="xMidYMid meet">
  <g transform="translate(0.000000,160.000000) scale(0.100000,-0.100000)" fill="${color}" stroke="none">
    <path d="M479 1225 c-15 -8 -32 -29 -38 -46 -7 -21 -11 -157 -11 -400 0 -367 0 -368 23 -394 43 -49 79 -45 218 27 172 90 488 283 522 318 35 39 36 87 2 128 -22 26 -148 105 -400 250 -114 65 -252 132 -272 132 -10 -1 -30 -7 -44 -15z"/>
  </g>
</svg>`;
  }
}

export interface DisplayedQuestionData {
  readonly question: Question;
  readonly questionIndex: number;
  readonly questionsCount: number;
  readonly nodeHeader: string;
  readonly isCustomQuiz?: boolean;
}
