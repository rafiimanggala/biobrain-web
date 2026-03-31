import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { CurrentUser } from 'src/app/auth/services/current-user';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveCourseService } from 'src/app/core/services/active-course.service';
import { QuizResultStreak } from 'src/app/core/services/quiz-result-streak/quiz-result-streak';
import { QuizResultStreakService } from 'src/app/core/services/quiz-result-streak/quiz-result-streak.service';
import { toNonNullableWithError } from '../../helpers/to-non-nullable';
import { StringsService } from '../../strings.service';

@Component({
  selector: 'app-quiz-result-history-sidenav',
  templateUrl: './quiz-result-history-sidenav.component.html',
  styleUrls: ['./quiz-result-history-sidenav.component.scss']
})
export class QuizResultHistorySidenavComponent extends BaseComponent implements OnInit, AfterViewInit {

  name: string = '';
  data$: Observable<QuizResultStreak>;

  constructor(
    public readonly strings: StringsService,
    quizResultStreakService: QuizResultStreakService,
    activeCourseService: ActiveCourseService,
    userService: CurrentUserService,
    appEvents: AppEventProvider
  ) {
    super(appEvents);
    this.subscriptions.push(userService.userChanges$.subscribe(this.setUser.bind(this)));

    this.data$ = activeCourseService.courseIdChanges$.pipe(
      map(toNonNullableWithError(this.strings.errors.courseMustBeSelected)),
      switchMap(courseId => quizResultStreakService.observeQuizResult(courseId))
    );

  }

  ngAfterViewInit(): void {
  }

  ngOnInit(): void {
  }

  private setUser(user: CurrentUser | undefined){
    this.name = user?.firstName ?? '';
  }
}
