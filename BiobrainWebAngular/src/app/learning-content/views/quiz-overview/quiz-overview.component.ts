import { Component, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { distinctUntilChanged, first, map, tap } from 'rxjs/operators';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveCourseService } from 'src/app/core/services/active-course.service';
import { StringsService } from 'src/app/share/strings.service';

import { QuizOverviewPageService } from './services/quiz-overview-page.service';
import { ThemeService } from 'src/app/core/app/theme.service';
import { QuestionType } from 'src/app/api/enums/question-type.enum';


@Component({
  selector: 'app-quiz-overview',
  templateUrl: './quiz-overview.component.html',
  styleUrls: ['./quiz-overview.component.scss'],
})
export class QuizOverviewComponent extends BaseComponent implements OnDestroy {

  courseId: string = '';
  subjectName$: Observable<string>;

  constructor(
    private readonly _routingService: RoutingService,
    private readonly _themeService: ThemeService,
    public strings: StringsService,
    public readonly quizOverviewPageService: QuizOverviewPageService,
    activatedRoute: ActivatedRoute,
    appEvents: AppEventProvider,
    activeCourseService: ActiveCourseService
  ) {
    super(appEvents);
    quizOverviewPageService.init(activatedRoute);

    this.subjectName$ = activeCourseService.courseChanges$.pipe(
      tap(x => { this.courseId = x?.courseId ?? ''; }),
      map(x => x?.subject?.name ?? ''),
      distinctUntilChanged(),
    );
    this.pushSubscribtions(this.quizOverviewPageService.questionClicked.subscribe(this.onQuestionClicked.bind(this)));
  }

  getHexColor(): string {
    return this._themeService.colors?.primary ?? "";
  }

  async onQuestionClicked(questionId: string){
    if (!this.quizOverviewPageService.data$) {
      return;
    }
    const data = await this.quizOverviewPageService.data$.pipe(first()).toPromise();
    var question = data.questions?.find(_ => _.questionId == questionId);
    if(!question) return;

    var element = document.getElementById(question.questionHeader);
    if(!element) return;
    element.scrollIntoView({
      behavior: "smooth",
      block: "start",
      inline: "nearest"
      });
  }

  getQuestionType(questionTypeCode: number): string {
    switch (questionTypeCode) {
      case QuestionType.multipleChoice:
        return this.strings.multipleChoice;
      case QuestionType.dropDown:
        return this.strings.dropDown;
      case QuestionType.freeText:
        return this.strings.freeText;
      case QuestionType.completeSentense:
        return this.strings.dropDown;
      case QuestionType.orderList:
        return this.strings.orderList;
      case QuestionType.swipe:
        return this.strings.swipe;
      case QuestionType.trueFalse:
        return this.strings.trueFalse;
      default: return "";
    }
  }

  onCourseClick() {
    if (!this.courseId) return;
    this._routingService.navigateToMaterialPage(this.courseId, undefined, undefined);
  }
}
