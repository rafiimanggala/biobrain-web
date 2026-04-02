import { stringify } from '@angular/compiler/src/util';
import { Component, ElementRef, Input, Renderer2, ViewEncapsulation } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Observable, ReplaySubject } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { removeTags } from 'src/app/share/helpers/regex-helper';

import { Question } from '../../../api/content/content-data-models';
import { ThemeService } from '../../../core/app/theme.service';
import { assertHasValue } from '../../../share/helpers/assert-has-value';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { rewriteImageUrls } from '../../../share/helpers/rewrite-image-urls';
import { RequestHandlingService } from '../../../share/services/request-parse.service';
import { StringsService } from '../../../share/strings.service';
import { ContentStyles } from '../../../share/values/content-styles';
import { PluggableScriptComponent } from '../learning-material-shadow-dom-node/pluggable-script.component';

@Component({
  selector: 'app-question-result-shadow-dom-node',
  templateUrl: './question-result-shadow-dom-node.component.html',
  styleUrls: ['./question-result-shadow-dom-node.component.scss'],
  encapsulation: ViewEncapsulation.ShadowDom
})
export class QuestionResultShadowDomNodeComponent extends PluggableScriptComponent {
  datum$ = new ReplaySubject<QuestionResultData>(1);
  formattedHtml$: Observable<SafeHtml>;
  private readonly _preventClickScriptId = 'preventClickScript';
  private readonly _shadowRoot: ShadowRoot;

  constructor(
    private readonly _element: ElementRef<HTMLDivElement>,
    private readonly _requestHandlingService: RequestHandlingService,
    private readonly _sanitizer: DomSanitizer,
    _renderer: Renderer2,
    private readonly _strings: StringsService,
    private readonly _themeService: ThemeService,
  ) {
    super(_renderer);
    assertHasValue(this._element.nativeElement.shadowRoot);
    this._shadowRoot = this._element.nativeElement.shadowRoot;

    this.formattedHtml$ = this.datum$.pipe(
      map(x => this._toFeedbackText(x)),
      switchMap(x => this._toStyledContent(x)),
      map(x => this._sanitizer.bypassSecurityTrustHtml(x))
    );
    this.includeScript(this._preventClickScriptId, "document.querySelector('app-question-result-shadow-dom-node').addEventListener('contextmenu', e => { e.preventDefault(); });");
  }

  @Input() set questionResult(value: QuestionResultData | null | undefined) {
    assertHasValue(value);
    this.datum$.next(value);
  }

  private async _toStyledContent(feedback: string): Promise<string> {
    const colors = await firstValueFrom(this._themeService.colors$);
    return rewriteImageUrls(`${ContentStyles.getAnswerStyles(colors)}${feedback}`);
  }

  private _toFeedbackText(datum: QuestionResultData): string {
    if (datum.isCorrect) {
      if (datum.question.feedBack) {
        return datum.question.feedBack;
      }

      return `<p>${this._strings.wellDone}</p>`;
    }

    if (datum.isSecondTry) {
      if (datum.question.feedBack) {
        return `<p>${datum.question.feedBack}</p>`;
      }
      return `<p>${this._strings.correctAnswerIs}: ${datum.question.answers.filter(_ => _.isCorrect).sort((a,b) => a.answerOrder - b.answerOrder).map(_ => _.text).toString()}</p>`;
    }

    if (removeTags(datum.question.hint)) {
      return datum.question.hint;
    }

    return `<p>${this._strings.tryAgain}</p>`;
  }

  protected getRootNode(): DocumentFragment {
    return this._shadowRoot;
  }
}

export interface QuestionResultData {
  readonly question: Question;
  readonly isCorrect: boolean;
  readonly isSecondTry: boolean;
}
