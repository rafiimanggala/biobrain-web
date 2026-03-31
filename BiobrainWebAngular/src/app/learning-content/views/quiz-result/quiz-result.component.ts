import { Component, OnDestroy } from '@angular/core';
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
import { QuizResult } from '../../../core/services/quizzes/quiz-result';
import { assertHasValue } from '../../../share/helpers/assert-has-value';

import { QuizResultPageService } from './services/quiz-result-page.service';

@Component({
  selector: 'app-quiz-result',
  templateUrl: './quiz-result.component.html',
  styleUrls: ['./quiz-result.component.scss'],
})
export class QuizResultComponent extends BaseComponent implements OnDestroy {
  
  courseId: string = '';
  subjectName$: Observable<string>;

  constructor(
    private readonly _routingService: RoutingService,
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
