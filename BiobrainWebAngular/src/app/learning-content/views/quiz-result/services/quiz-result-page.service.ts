import { Injectable } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { combineLatest, Observable } from 'rxjs';
import { map, distinctUntilChanged, shareReplay, switchMap } from 'rxjs/operators';
import { Question } from 'src/app/api/content/content-data-models';
import { QuestionType } from 'src/app/api/enums/question-type.enum';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { ContentTreeNode } from 'src/app/core/services/content/content-tree.node';
import { ContentTreeService } from 'src/app/core/services/content/content-tree.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { QuizResult } from 'src/app/core/services/quizzes/quiz-result';
import { QuizResultService } from 'src/app/core/services/quizzes/quiz-result.service';
import { SidenavService } from 'src/app/core/services/side-nav.service';
import { LearningContentProviderService } from 'src/app/learning-content/services/learning-content-provider.service';
import { distinct } from 'src/app/share/helpers/distinct-arrays';
import { toNonNullableWithError } from 'src/app/share/helpers/to-non-nullable';
import { RequestHandlingService } from 'src/app/share/services/request-parse.service';
import { StringsService } from 'src/app/share/strings.service';
import { Colors } from 'src/app/share/values/colors';
import { ContentStyles } from 'src/app/share/values/content-styles';

import { ThemeService } from '../../../../core/app/theme.service';
import { QuestionResult } from '../../../../share/components/hexagone-results/models/question-result.model';
import { firstValueFrom } from '../../../../share/helpers/first-value-from';
import { hasValue } from '../../../../share/helpers/has-value';

@Injectable()
export class QuizResultPageService extends DisposableSubscriptionService {

  public data$?: Observable<{ quizResult: QuizResult; question: Question | undefined; text: SafeHtml | null; feedback: SafeHtml | null; questionHeader: string; correctAnswer: SafeHtml | null; path: string; reuslts: QuestionResult[]; successRate: number; node: ContentTreeNode }>;

  constructor(
    public strings: StringsService,
    private readonly _sidenavService: SidenavService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _routingService: RoutingService,
    private readonly _quizResultService: QuizResultService,
    private readonly _learningContentProvider: LearningContentProviderService,
    private readonly _appEvents: AppEventProvider,
    private readonly _sanitizer: DomSanitizer,
    private readonly _themeService: ThemeService,
    public readonly requestService: RequestHandlingService,
  ) {
    super();
  }

  init(activatedRoute: ActivatedRoute) {

    const questionId$ = activatedRoute.paramMap.pipe(
      map(_ => _.get('questionId')),
      distinctUntilChanged(),
      shareReplay(1),
    );

    const quizResultId$ = activatedRoute.paramMap.pipe(
      map(_ => _.get('quizResultId')),
      map(toNonNullableWithError(this.strings.errors.routeParameterWasNotFound('quizResultId'))),
      distinctUntilChanged(),
      shareReplay(1)
    );

    const quizResult$ = quizResultId$.pipe(
      switchMap(quizResultId => this._quizResultService.observeQuizResult(quizResultId)),
      shareReplay(1),
    );

    const quiz$ = quizResult$.pipe(
      map(_ => _.quizId),
      distinctUntilChanged(),
      switchMap(quizId => this._learningContentProvider.getQuizById(quizId)),
      map(toNonNullableWithError(this.strings.quizNotFoundMessage)),
      shareReplay(1),
    );

    // const onInitTree$ = this._activeCourseService.courseIdChanges$.pipe(
    //     map(toNonNullableWithError(this.strings.subjectNotSelectedMessage)),
    //     switchMap(courseId => this._treeService.initTree(courseId, this._module)),
    // );

    const contentTreeNode$ = quiz$.pipe(
      switchMap(quiz => this._contentTreeService.getNode(quiz.contentTreeNodeId)),
      shareReplay(1),
    );

    const question$ = combineLatest([quiz$, questionId$]).pipe(
      map(([quiz, questionId]) => quiz.questions.find(_ => _.questionId === questionId)),
      shareReplay(1),
    );

    this.data$ = combineLatest([question$, quiz$, quizResult$, contentTreeNode$]).pipe(
      switchMap(async ([question, quiz, quizResult, node]) => ({
        quizResult,
        question,
        questionHeader: `Q${question ? quizResult.questions.findIndex(x => x.questionId == question.questionId) + 1 : 0}`,
        text: await this.transformQuestionText(question),
        feedback: await this.transformQuestionFeedback(question),
        correctAnswer: this.getCorrectAnswer(question),
        path: node.fullName,
        reuslts: quizResult.questions.map(x => new QuestionResult(
          x.questionId,
          this.strings.questionPrefix + (quizResult.questions.indexOf(x) + 1).toString(),
          x.isCorrect ? Colors.green : Colors.red)
        ),
        node,
        successRate: quizResult.score/100
      }))
    );

    this.subscriptions.push(
      combineLatest([quizResult$, quiz$, questionId$]).subscribe(
        ([_quizResult, _quiz, questionId]) => {
          if (!hasValue(questionId)) this._sidenavService.open();
        },
        error => {
          this._appEvents.errorEmit(error);
          console.error(error);
          void this._routingService.navigateToHome();
        },
      ),
    );
  }

  async transformQuestionText(question: Question | undefined): Promise<SafeHtml | null> {
    if (!question) return null;
    let { text } = question;
    let pattern;
    switch (Number(question.questionTypeCode)) {
      case QuestionType.multipleChoice:
      case QuestionType.trueFalse:
      case QuestionType.freeText:
        pattern = /<form[\S\s]+<\/form>/g;
        text = text.replace(pattern, '');
        return this._sanitizer.bypassSecurityTrustHtml(await this._getText(text));
      case QuestionType.dropDown:
      case QuestionType.completeSentense:
        pattern = /<\s*span\s*class="selectBox( wide)?"\s*>([\S\s]+?)<\/span>/g;
        text = text.replace(pattern, '______');
        text = this._removeAnswerButton(text);
        return this._sanitizer.bypassSecurityTrustHtml(await this._getText(text));
      case QuestionType.orderList:
        console.log(question.text);
        pattern = /<\s*div\s*class="container"\s*>([\S\s]+?)<\/div>/g;
        text = text.replace(pattern, '');
        text = this._removeAnswerButton(text);
        return this._sanitizer.bypassSecurityTrustHtml(await this._getText(text));
      case QuestionType.swipe:
        pattern = /<div[^>]*?class="container"[^>]*?>(.*?)<\/div>/g;
        text = text.replace(pattern, '');
        text = this._removeAnswerButton(text);
        return this._sanitizer.bypassSecurityTrustHtml(await this._getText(text));
    }
    return null;
  }

  async transformQuestionFeedback(question: Question | undefined): Promise<SafeHtml | null> {
    if (!question) return null;
    return this._sanitizer.bypassSecurityTrustHtml(await this._getFeedback(question.feedBack));
  }

  getCorrectAnswer(question: Question | undefined): SafeHtml | null {
    if (!question) return null;
    const { text } = question;
    switch (Number(question.questionTypeCode)) {
      case QuestionType.multipleChoice:
      case QuestionType.trueFalse:
      case QuestionType.freeText:
        return this._sanitizer.bypassSecurityTrustHtml(question.answers.find(x => x.isCorrect)?.text ?? '');
      case QuestionType.dropDown:
      case QuestionType.completeSentense: {
        const resposes = distinct(question.answers.map(x => x.response));
        let answer = '';
        resposes.sort((a, b) => a - b).forEach(x => {
          answer = answer + `<b>${this.strings.response} ${x}:</b> `;
          question.answers.filter(a => a.response === x && a.isCorrect).forEach(a => {
            answer = answer + `${a.text}, `;
          });
          answer = answer.substring(0, answer.length - 2);
          answer = answer + ' ';
        });
        return this._sanitizer.bypassSecurityTrustHtml(answer);
      }
      case QuestionType.orderList: {
        let answer = '';
        question.answers.sort((a,b) => a.response - b.response).forEach(x => {
          answer = answer + `${x.response}: ${x.text} <br>`;
        });
        return this._sanitizer.bypassSecurityTrustHtml(answer);
      }
      case QuestionType.swipe: {
        const leftGroupPattern = /<[Bb]utton.*?class="leftColumnHeader".*?>(.*?)<\/[Bb]utton>/g;
        const rigntGroupPattern = /<[Bb]utton.*?class="rightColumnHeader".*?>(.*?)<\/[Bb]utton>/g;
        let match = leftGroupPattern.exec(question.text);
        const leftGroup = match ? match[1] : '';
        match = rigntGroupPattern.exec(question.text);
        const rightGroup = match ? match[1] : '';
        let answer = '';
        answer = answer + `<b>${leftGroup}:</b> `;
        question.answers.filter(a => a.response === 1 && a.isCorrect).forEach(a => {
          answer = answer + `${a.text}, `;
        });
        answer = answer.substring(0, answer.length - 2);
        answer = answer + ' ';
        answer = answer + `<b>${rightGroup}:</b> `;
        question.answers.filter(a => a.response === 2 && a.isCorrect).forEach(a => {
          answer = answer + `${a.text}, `;
        });
        return this._sanitizer.bypassSecurityTrustHtml(answer);
      }
    }
    return null;
  }

  buttonPattern = /<[Bb]utton.*?class="answerButton".*?>(.*?)<\/[Bb]utton>/g;
  private _removeAnswerButton(html: string): string {
    return html.replace(this.buttonPattern, '');
  }

  private async _getText(questionText: string): Promise<string> {
    const colors = await firstValueFrom(this._themeService.colors$);
    return `<html>${ContentStyles.getQuestionStyles(colors)}<section><article class="questionText">${questionText}</article></section></html>`;
  }

  private async _getFeedback(feedback: string): Promise<string> {
    const colors = await firstValueFrom(this._themeService.colors$);
    return `<html>${ContentStyles.getAnswerStyles(colors)}<section><article  class="questionText">${feedback}</article></section></html>`;
  }
}
