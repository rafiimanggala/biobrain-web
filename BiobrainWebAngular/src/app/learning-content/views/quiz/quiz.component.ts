import { Component, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BehaviorSubject, combineLatest, Observable, Subscription } from 'rxjs';
import { distinctUntilChanged, filter, map, shareReplay, switchMap, tap } from 'rxjs/operators';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveSchoolClassService } from 'src/app/core/services/active-school-class.service';
import { ActiveSchoolService } from 'src/app/core/services/active-school.service';
import { StringsService } from 'src/app/share/strings.service';
import { AppSettings } from 'src/app/share/values/app-settings';

import { Question } from '../../../api/content/content-data-models';
import { Dialog } from '../../../core/dialogs/dialog.service';
import { QuizResult } from '../../../core/services/quizzes/quiz-result';
import { QuizResultService } from '../../../core/services/quizzes/quiz-result.service';
import { QuizSoundService } from '../../../core/services/quiz-sound.service';
import { QuizzesService } from '../../../core/services/quizzes/quizzes.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { randomItemForArray } from '../../../share/helpers/random-item-for-array';
import { toNonNullableWithError } from '../../../share/helpers/to-non-nullable';
import { LoaderService } from '../../../share/services/loader.service';
import { QuestionResultDialogData } from '../../dialogs/question-result-dialog/question-result-dialog-data';
import { QuestionResultDialogComponent } from '../../dialogs/question-result-dialog/question-result-dialog.component';
import { CorrectnessService } from '../../services/correctness.service';

@Component({
  selector: 'app-quiz',
  templateUrl: './quiz.component.html',
  styleUrls: ['./quiz.component.scss'],
  providers: [CorrectnessService],
})
export class QuizComponent extends BaseComponent implements OnDestroy {
  public readonly data$: Observable<{ quizResult: QuizResult; question: Question; questionIndex: number; totalQuestions: number; nodeHeader: string; path: string; hintsEnabled: boolean; soundEnabled: boolean }>;
  public readonly questionHasAnswer$: Observable<boolean>;
  public readonly attemptCounts$ = new BehaviorSubject<number>(0);
  public studentSoundOverride: boolean | null = null;

  private readonly _subscriptions: Subscription[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _routingService: RoutingService,
    private readonly _quizzesService: QuizzesService,
    private readonly _quizResultService: QuizResultService,
    private readonly _dialog: Dialog,
    private readonly _correctnessService: CorrectnessService,
    private readonly _quizSoundService: QuizSoundService,
    private readonly _loaderService: LoaderService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _activeSchoolService: ActiveSchoolService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);

    const questionId$ = this._activatedRoute.paramMap.pipe(
      map(_ => _.get('questionId')),
      distinctUntilChanged(),
      shareReplay({
        bufferSize: 1,
        refCount: true
      }),
    );

    const quizResultId$ = this._activatedRoute.paramMap.pipe(
      map(_ => _.get('quizResultId')),
      map(toNonNullableWithError(this.strings.errors.routeParameterWasNotFound('quizResultId'))),
      distinctUntilChanged(),
      shareReplay({
        bufferSize: 1,
        refCount: true
      }),
    );

    const quizResultLoadingProcess$ = quizResultId$.pipe(
      switchMap(quizResultId => this._quizResultService.observeQuizLoadingProcess(quizResultId)),
      shareReplay({
        bufferSize: 1,
        refCount: true
      }),
    );

    const quizResult$ = quizResultLoadingProcess$.pipe(switchMap(_ => _.data$));

    const quiz$ = quizResult$.pipe(
      map(_ => _.quizId),
      distinctUntilChanged(),
      switchMap(quizId => this._quizzesService.getById(quizId)),
      map(toNonNullableWithError(this.strings.quizNotFoundMessage)),
      shareReplay({
        bufferSize: 1,
        refCount: true
      }),
    );

    const question$ = combineLatest([quiz$, questionId$.pipe(filter(hasValue))]).pipe(
      map(([quiz, questionId]) => quiz.row.questions.find(_ => _.questionId === questionId)),
      map(toNonNullableWithError(strings.errors.questionWasNotFound)),
      tap(_ => this.attemptCounts$.next(0)),
      shareReplay({
        bufferSize: 1,
        refCount: true
      }),
    );

    this.questionHasAnswer$ = combineLatest([quizResult$, question$]).pipe(
      map(([quizResult, question]) => quizResult.questions.some(_ => _.questionId === question.questionId)),
      shareReplay({
        bufferSize: 1,
        refCount: true
      })
    );

    this.data$ = combineLatest([question$, quiz$, quizResult$]).pipe(
      map(([question, quiz, quizResult]) => ({
        quizResult,
        question,
        questionIndex:
          quizResult.questions.findIndex(_ => _.questionId === question.questionId) < 0
            ? quizResult.questions.length + 1
            : quizResult.questions.findIndex(_ => _.questionId === question.questionId) + 1,
        totalQuestions: quiz.row.questions.filter(q => !quizResult.excludedQuestions.some(_ => _ === q.questionId)).length,
        nodeHeader: quiz.node.row.name ?? '',
        path: quiz.fullName,
        hintsEnabled: quizResult.hintsEnabled,
        soundEnabled: quizResult.soundEnabled,
      })),
    );

    this._subscriptions.push(
      combineLatest([quizResult$, quiz$, questionId$, this.attemptCounts$]).subscribe(
        ([quizResult, quiz, questionId, attemptCounts]) => {
          const unansweredQuestions = quiz.row.questions.filter(q => !quizResult.questions.some(_ => _.questionId === q.questionId) && !quizResult.excludedQuestions.some(_ => _ === q.questionId));

          if (hasValue(questionId)) {
            const questionHasAnswer = quizResult.questions.some(_ => _.questionId === questionId);
            if (!questionHasAnswer || attemptCounts < 2) {
              return;
            }
          }

          if (quizResult.questions.length >= AppSettings.quizQuestionsNumber || unansweredQuestions.length === 0) {
            this._finishQuiz(quizResult.quizResultId);
            return;
          }

          if (quizResult.schoolClassId) {
            this._activeSchoolClassService.setActiveSchoolClassId(quizResult.schoolClassId);
          }

          if (quizResult.schoolId) {
            this._activeSchoolService.setSchoolId(quizResult.schoolId);
          }

          if (quizResult.schoolName) {
            this._activeSchoolService.setSchoolName(quizResult.schoolName);
          }

          this. _showNextQuestion(quizResult, unansweredQuestions);
        },
        error => {
          this.error(error);
          void this._routingService.navigateToHome();
        },
      ),

      quizResultLoadingProcess$.pipe(switchMap(_ => _.running$)).subscribe(
        loading => loading ? this._loaderService.show() : this._loaderService.hideIfVisible(),
        _ => this._loaderService.hideIfVisible(),
        () => this._loaderService.hideIfVisible(),
      ),
    );
  }

  ngOnDestroy(): void {
    this._subscriptions.forEach(_ => _.unsubscribe());
  }

  public toggleSound(currentSoundEnabled: boolean): void {
    const current = this.studentSoundOverride !== null
      ? this.studentSoundOverride
      : currentSoundEnabled;
    this.studentSoundOverride = !current;
  }

  public getEffectiveSoundEnabled(serverSoundEnabled: boolean): boolean {
    return this.studentSoundOverride !== null
      ? this.studentSoundOverride
      : serverSoundEnabled;
  }

  public async onAnswerValueChange(quizResult: QuizResult, question: Question, answerValue: string): Promise<void> {
    const correctness = this._correctnessService.checkIsCorrect(question, answerValue);

    const effectiveSoundEnabled = this.studentSoundOverride !== null
      ? this.studentSoundOverride
      : quizResult.soundEnabled;
    this._quizSoundService.setSoundEnabled(effectiveSoundEnabled);
    if (correctness.isCorrect) {
      this._quizSoundService.playCorrect();
    } else {
      this._quizSoundService.playIncorrect();
    }

    const dialogData = new QuestionResultDialogData(correctness.isCorrect, this.attemptCounts$.value !== 0, question);
    await this._dialog.show(QuestionResultDialogComponent, dialogData, { panelClass: correctness.isCorrect ? "correct-question-result-dialog-panel" : "incorrect-question-result-dialog-panel" });

    if (!await firstValueFrom(this.questionHasAnswer$)) {
      try {
        this._loaderService.show();
        await this._quizResultService.saveAnswerValue(quizResult.quizResultId, question.questionId, answerValue, correctness.isCorrect);
      } finally {
        this._loaderService.hide();
      }
    }

    if (correctness.isCorrect) {
      this.attemptCounts$.next(2);
    } else {
      this.attemptCounts$.next(this.attemptCounts$.value + 1);
    }
  }

  private _showNextQuestion(quizResult: QuizResult, questions: Question[]): void {
    void this._routingService.navigateToQuizPage(quizResult.quizResultId, randomItemForArray(questions).questionId, { replaceUrl: true });
  }

  private _finishQuiz(quizResultId: string): void {
    void this._routingService.navigateToQuizResultPage(quizResultId, undefined, { replaceUrl: true });
  }
}
