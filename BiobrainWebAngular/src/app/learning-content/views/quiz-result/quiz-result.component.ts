import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { distinctUntilChanged, map, tap } from 'rxjs/operators';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveCourseService } from 'src/app/core/services/active-course.service';
import { StringsService } from 'src/app/share/strings.service';
import { Colors } from 'src/app/share/values/colors';

import { Question } from '../../../api/content/content-data-models';
import { Dialog } from '../../../core/dialogs/dialog.service';
import { DialogAction } from '../../../core/dialogs/dialog-action';
import { QuizResult } from '../../../core/services/quizzes/quiz-result';
import { assertHasValue } from '../../../share/helpers/assert-has-value';
import { StarRatingDialogComponent } from '../../../share/dialogs/star-rating-dialog/star-rating-dialog.component';
import { StarRatingDialogData } from '../../../share/dialogs/star-rating-dialog/star-rating-dialog-data';

import { QuizResultPageService } from './services/quiz-result-page.service';

@Component({
  selector: 'app-quiz-result',
  templateUrl: './quiz-result.component.html',
  styleUrls: ['./quiz-result.component.scss'],
})
export class QuizResultComponent extends BaseComponent implements OnDestroy, OnInit {
  private static readonly QUIZ_COUNT_KEY = 'biobrain_quiz_completion_count';
  private static readonly HAS_RATED_KEY = 'biobrain_has_rated';
  private static readonly RATING_TRIGGERS = [3, 8, 15];

  courseId: string = '';
  subjectName$: Observable<string>;

  constructor(
    private readonly _routingService: RoutingService,
    private readonly _dialog: Dialog,
    public strings: StringsService,
    public readonly quizResultPageService: QuizResultPageService,
    activatedRoute: ActivatedRoute,
    appEvents: AppEventProvider,
    activeCourseService: ActiveCourseService
  ) {
    super(appEvents);
    quizResultPageService.init(activatedRoute);

    this.subjectName$ = activeCourseService.courseChanges$.pipe(
      tap(x => {this.courseId = x?.courseId ?? '';}),
      map(x => x?.subject?.name ?? ''),
      distinctUntilChanged(),
    );
  }

  ngOnInit(): void {
    void this._checkAndShowRatingDialog();
  }

  private async _checkAndShowRatingDialog(): Promise<void> {
    const hasRated = localStorage.getItem(QuizResultComponent.HAS_RATED_KEY) === 'true';
    if (hasRated) return;

    const currentCount = parseInt(localStorage.getItem(QuizResultComponent.QUIZ_COUNT_KEY) || '0', 10);
    const newCount = currentCount + 1;
    localStorage.setItem(QuizResultComponent.QUIZ_COUNT_KEY, String(newCount));

    if (!QuizResultComponent.RATING_TRIGGERS.includes(newCount)) return;

    const result = await this._dialog.show(
      StarRatingDialogComponent,
      new StarRatingDialogData('How are you finding BioBrain?'),
      { disableClose: false, width: '420px' }
    );

    if (result && result.action === DialogAction.save) {
      localStorage.setItem(QuizResultComponent.HAS_RATED_KEY, 'true');
    }
  }

  getHexColor(data: { quizResult: QuizResult; question: Question | undefined }): string {
    return this._isCorrect(data) ? Colors.green : Colors.red;
  }

  private _isCorrect(data: { quizResult: QuizResult; question: Question | undefined }): boolean {
    const { quizResult, question } = data;
    assertHasValue(question);

    const questionResult = quizResult.questions.find(_ => _.questionId === question.questionId);
    if (!questionResult) {
      return false;
    }

    return questionResult.isCorrect;
  }

  onCourseClick(){
    if (!this.courseId) return;
    this._routingService.navigateToMaterialPage(this.courseId, undefined, undefined);}
}
