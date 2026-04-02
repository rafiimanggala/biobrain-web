import { TitleCasePipe } from '@angular/common';
import { EventEmitter, Injectable } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { combineLatest, Observable } from 'rxjs';
import { map, distinctUntilChanged, shareReplay, switchMap, tap } from 'rxjs/operators';
import { Api } from 'src/app/api/api.service';
import { Question } from 'src/app/api/content/content-data-models';
import { QuestionType } from 'src/app/api/enums/question-type.enum';
import { AddExcludedQuestionCommand } from 'src/app/api/quiz-assignments/add-excluded-question.command';
import { GetExcludedQuestionsQuery } from 'src/app/api/quiz-assignments/get-excluded-questions.query';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { ActiveSchoolClassService } from 'src/app/core/services/active-school-class.service';
import { ContentTreeNode } from 'src/app/core/services/content/content-tree.node';
import { ContentTreeService } from 'src/app/core/services/content/content-tree.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { SidenavService } from 'src/app/core/services/side-nav.service';
import { LearningContentProviderService } from 'src/app/learning-content/services/learning-content-provider.service';
import { QuestionViewModel } from 'src/app/share/components/hexagone-questions/models/question.model';
import { distinct } from 'src/app/share/helpers/distinct-arrays';
import { toNonNullableWithError } from 'src/app/share/helpers/to-non-nullable';
import { LoaderService } from 'src/app/share/services/loader.service';
import { RequestHandlingService } from 'src/app/share/services/request-parse.service';
import { StringsService } from 'src/app/share/strings.service';
import { ContentStyles } from 'src/app/share/values/content-styles';

import { ThemeService } from '../../../../core/app/theme.service';
import { firstValueFrom } from '../../../../share/helpers/first-value-from';
import { hasValue } from '../../../../share/helpers/has-value';
import { rewriteImageUrls } from '../../../../share/helpers/rewrite-image-urls';
import { GetQuizFullnessStatusQuery, GetQuizFullnessStatusQuery_Result } from 'src/app/api/quiz-assignments/get-quiz-fullness-status.query';
import { QuestionTextService } from 'src/app/share/services/question-text.service';

@Injectable()
export class QuizOverviewPageService extends DisposableSubscriptionService {

  public dataRefreshed: EventEmitter<string> = new EventEmitter<string>();
  public questionClicked: EventEmitter<string> = new EventEmitter<string>();
  public data$?: Observable<{ questions: QuestionViewModel[]|null; path: string; node: ContentTreeNode, isFull: boolean }>;
  public quizId = '';
  public schoolClassId = '';

  constructor(
    public strings: StringsService,
    private readonly _sidenavService: SidenavService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _routingService: RoutingService,
    private readonly _learningContentProvider: LearningContentProviderService,
    private readonly _appEvents: AppEventProvider,
    private readonly _sanitizer: DomSanitizer,
    private readonly _themeService: ThemeService,
    private readonly _titleCasePipe: TitleCasePipe,
    public readonly requestService: RequestHandlingService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _questionTextService: QuestionTextService,
    private readonly _loaderService: LoaderService,
    private readonly _api: Api,
  ) {
    super();
  }

  init(activatedRoute: ActivatedRoute): any {

    const quizId$ = activatedRoute.paramMap.pipe(
      map(_ => _.get('quizId')),
      map(toNonNullableWithError(this.strings.errors.routeParameterWasNotFound('quizId'))),
      distinctUntilChanged(),
      shareReplay(1)
    );

    const quiz$ = quizId$.pipe(
      distinctUntilChanged(),
      switchMap(quizId => this._learningContentProvider.getQuizById(quizId)),
      map(toNonNullableWithError(this.strings.quizNotFoundMessage)),
      tap(_ => this.quizId = _.quizId),
      shareReplay(1),
    );

    const contentTreeNode$ = quiz$.pipe(
      switchMap(quiz => this._contentTreeService.getNode(quiz.contentTreeNodeId)),
      shareReplay(1),
    );

    const questions$ = quiz$.pipe(
      switchMap(quiz => this.getQuestions(quiz.questions, quiz.quizId)),
      shareReplay(1),
    );

    const quizFullnes$ = quiz$.pipe(
      switchMap(quiz => this.getQuizFullnessStatus(quiz.quizId)),
      shareReplay(1),
    );

    this.data$ = combineLatest([questions$, contentTreeNode$, quizFullnes$]).pipe(
      // tap(_ => console.log(_)),
      switchMap(async ([questions, node, fullness]) => ({
        questions,
        path: node.fullName,
        node,
        isFull: fullness.isQuizFull
      })),
      tap(_ => console.log(_))
    );

    this.subscriptions.push(
      combineLatest([this.data$]).subscribe(
        ([data]) => {
          if (!hasValue(data)) this._sidenavService.open();
        },
        error => {
          this._appEvents.errorEmit(error);
          console.error(error);
          void this._routingService.navigateToHome();
        },
      ),
    );
  }

  async getQuestions(questions: Question[], quizId: string): Promise<QuestionViewModel[] | null> {
    if (!questions) return null;
    this.schoolClassId = await this._activeSchoolClassService.schoolClassId ?? '';
    if (!hasValue(this.schoolClassId)) return null;

    const excludedQuestionsId = await this.getExcludedQuestions(this.schoolClassId, quizId) ?? [];

    const models = [];
    for (const question of questions.sort((a,b) => Number(a.header.substring(1)) - Number(b.header.substring(1)))) {
      const questionHeader = question.header;
      const text = await this._questionTextService.transformQuestionTextSafe(question);
      const feedback = await this.transformQuestionFeedback(question);
      const correctAnswer = this.getCorrectAnswer(question);
      const answers = [];
      for (const answer of question.answers.sort((a,b) => a.response - b.response)) {
        const answerText = await this.transformQuestionAnswer(answer.text);
        if (answerText == null) continue;
        answers.push(answerText);
      }
      models.push({
        text,
        feedback,
        questionHeader,
        correctAnswer,
        answers,
        questionTypeCode: Number(question.questionTypeCode),
        questionId: question.questionId,
        isExcluded: excludedQuestionsId.some(_ => _ == question.questionId)
      });
    }
    return models;
  }

  async getExcludedQuestions(schoolClassId: string, quizId: string): Promise<string[]> {
    //
    try {
      this._loaderService.show();
      const result = await firstValueFrom(this._api.send(new GetExcludedQuestionsQuery(schoolClassId, quizId)));
      return result.questionIds;
    } catch (e: any) {
      // this._appEventsService.errorEmit(e.message);
      return [];
    } finally {
      this._loaderService.hideIfVisible();
    }
  }

  async getQuizFullnessStatus(quizId: string): Promise<GetQuizFullnessStatusQuery_Result> {
    //
    try {
      this._loaderService.show();
      const result = await firstValueFrom(this._api.send(new GetQuizFullnessStatusQuery( quizId)));
      return result;
    } catch (e: any) {
      // this._appEventsService.errorEmit(e.message);
      return {isQuizFull: false};
    } finally {
      this._loaderService.hideIfVisible();
    }
  }

  async removeAddBackQuestion(question: QuestionViewModel) {
    //
    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new AddExcludedQuestionCommand(this.schoolClassId, this.quizId, question.questionId, !question.isExcluded)));
      question.isExcluded = !question.isExcluded;
      this.dataRefreshed.emit('questions');
    } catch (e: any) {
      // this._appEventsService.errorEmit(e.message);
    } finally {
      this._loaderService.hideIfVisible();
    }
  }

  

  async transformQuestionFeedback(question: Question | undefined): Promise<SafeHtml | null> {
    if (!question) return null;
    return this._sanitizer.bypassSecurityTrustHtml(await this._getFeedback(question.feedBack));
  }

  async transformQuestionAnswer(answer: string | undefined): Promise<SafeHtml | null> {
    if (!answer) return null;
    return this._sanitizer.bypassSecurityTrustHtml(await this._getAnswer(answer));
  }

  getCorrectAnswer(question: Question | undefined): SafeHtml | null {
    if (!question) return null;
    const { text } = question;
    switch (Number(question.questionTypeCode)) {
      case QuestionType.trueFalse:
        return this._sanitizer.bypassSecurityTrustHtml(this._titleCasePipe.transform(question.answers.find(x => x.isCorrect)?.text ?? ''));
      case QuestionType.multipleChoice:
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

 

  private async _getFeedback(feedback: string): Promise<string> {
    const colors = await firstValueFrom(this._themeService.colors$);
    return rewriteImageUrls(`<html>${ContentStyles.getAnswerStyles(colors)}<section><article class="questionText">${feedback}</article></section></html>`);
  }

  private async _getAnswer(answer: string): Promise<string> {
    const colors = await firstValueFrom(this._themeService.colors$);
    return rewriteImageUrls(`<html>${ContentStyles.getAnswerStyles(colors)}<section><article>${answer}</article></section></html>`);
  }
}
