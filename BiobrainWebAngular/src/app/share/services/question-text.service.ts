import { Injectable } from "@angular/core";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { Question } from "src/app/api/content/content-data-models";
import { QuestionType } from "src/app/api/enums/question-type.enum";
import { ContentStyles } from "../values/content-styles";
import { firstValueFrom } from "../helpers/first-value-from";
import { ThemeService } from "src/app/core/app/theme.service";

@Injectable({
    providedIn: 'root',
})
export class QuestionTextService{
    private readonly _buttonsQuestionsRegEx = /(<button .*?)(type=["']submit["'].*?)(value=["'](.*?)["'])(.*?>.*?\n*?.*?<\/button>)/g;
    private readonly _buttonsQuestionsExpression = "$1 type=\"button\" onclick=\"sendRequest(event, 'answer.response1=$4')\"$5";
    private readonly _columnHeadersRegEx = /<center>(<button .*?)(type=["']submit["'].*?)(value=["'](.*?)["'])(.*?>.*?\n*?.*?<\/button>)<\/center>/g;
    private readonly _columnHeadersExpression = '<div class="columnHeaders">$1$2$3$5</div>';

    constructor(
        private readonly _sanitizer: DomSanitizer,
        private readonly _themeService: ThemeService,
    ){}

    
    async transformQuestionText(question: Question | undefined): Promise<string | null> {
        if (!question) return null;
        let { text } = question;
        let pattern;
        switch (Number(question.questionTypeCode)) {
          case QuestionType.multipleChoice:
          case QuestionType.trueFalse:
            // pattern = /<form[\S\s]+<\/form>/g;
            text = text.replace(this._buttonsQuestionsRegEx, this._buttonsQuestionsExpression);
            return await this._getText(text);
          case QuestionType.freeText:
            pattern = /<form[\S\s]+<\/form>/g;
            text = text.replace(pattern, '');
            return await this._getText(text);
          case QuestionType.dropDown:
          case QuestionType.completeSentense:
            pattern = /<\s*span\s*class="selectBox( wide)?"\s*>([\S\s]+?)<\/span>/g;
            text = text.replace(pattern, '______');
            text = this._removeAnswerButton(text);
            return await this._getText(text);
          case QuestionType.orderList:
            // pattern = /<\s*div\s*class="container"\s*>([\S\s]+?)<\/div>/g;
            // text = text.replace(pattern, '');
            text = this._removeAnswerButton(text);
            return await this._getText(text);
          case QuestionType.swipe:
            // pattern = /<div[^>]*?class="container"[^>]*?>(.*?)<\/div>/g;
            // text = text.replace(pattern, '');
            text = text.replace(this._columnHeadersRegEx, this._columnHeadersExpression);
            text = this._removeAnswerButton(text);
            return await this._getText(text);
        }
        return null;
    }

    async transformQuestionTextSafe(question: Question | undefined): Promise<SafeHtml | null> {
        let text = await this.transformQuestionText(question);
        return text ? this._sanitizer.bypassSecurityTrustHtml(text) : null;
    }

    buttonPattern = /<[Bb]utton[^>]*?class="answerButton".*?>(.*?)<\/[Bb]utton>/g;
    private _removeAnswerButton(html: string): string {
      return html.replace(this.buttonPattern, '');
    }
  
    private async _getText(questionText: string): Promise<string> {
      const colors = await firstValueFrom(this._themeService.colors$);
      return `<html>${ContentStyles.getQuestionStyles(colors)}<section><article class="questionText">${questionText}</article></section></html>`;
    }
}