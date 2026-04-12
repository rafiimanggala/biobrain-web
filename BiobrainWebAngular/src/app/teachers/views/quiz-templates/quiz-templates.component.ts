import { Component, OnInit } from '@angular/core';
import { combineLatest, Observable } from 'rxjs';
import { distinctUntilChanged, filter, map, switchMap, tap } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import {
  CreateQuizFromTemplateCommand,
} from '../../../api/quizzes/create-quiz-from-template.command';
import {
  GetQuizTemplatesQuery,
  QuizTemplate,
} from '../../../api/quizzes/get-quiz-templates.query';
import { CurrentUserService } from '../../../auth/services/current-user.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { BaseComponent } from '../../../core/app/base.component';
import { Dialog } from '../../../core/dialogs/dialog.service';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { TeacherCourseGroup } from '../../../core/services/courses/teacher-course-group';
import { TeacherCoursesService } from '../../../core/services/courses/teacher-courses.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { SnackBarService } from '../../../share/services/snack-bar.service';
import { StringsService } from '../../../share/strings.service';
import { SelectClassDialog } from '../../dialogs/select-class-dialog/select-class-dialog.component';
import { SelectClassDialogData } from '../../dialogs/select-class-dialog/select-class-dialog.data';

@Component({
  selector: 'app-quiz-templates',
  templateUrl: './quiz-templates.component.html',
  styleUrls: ['./quiz-templates.component.scss'],
})
export class QuizTemplatesComponent extends BaseComponent implements OnInit {
  templates: QuizTemplate[] = [];
  displayedColumns: string[] = ['name', 'questionCount', 'hints', 'sound', 'createdAt', 'actions'];

  private _userId = '';
  private _courseId = '';
  private _courseGroups: TeacherCourseGroup[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _currentUserService: CurrentUserService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _teacherCoursesService: TeacherCoursesService,
    private readonly _dialog: Dialog,
    private readonly _snackBarService: SnackBarService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);
  }

  ngOnInit(): void {
    const userId$ = this._currentUserService.userChanges$.pipe(
      filter(hasValue),
      map(user => user.userId),
      tap(userId => this._userId = userId),
      distinctUntilChanged(),
    );

    const courseId$ = this._activeCourseService.courseIdChanges$.pipe(
      filter(hasValue),
      tap(courseId => this._courseId = courseId),
      distinctUntilChanged(),
    );

    this.pushSubscribtions(
      combineLatest([userId$, courseId$]).pipe(
        tap(() => this.startLoading()),
        switchMap(([userId, courseId]) => this._api.send(new GetQuizTemplatesQuery(userId, courseId))),
        tap(result => {
          this.templates = result.templates;
          this.endLoading();
        }),
      ).subscribe(),

      this._currentUserService.userChanges$.pipe(
        filter(hasValue),
        switchMap(user => this._teacherCoursesService.getTeacherCourses(user.userId)),
        tap(groups => this._courseGroups = groups),
      ).subscribe(),
    );
  }

  async onUseTemplate(template: QuizTemplate): Promise<void> {
    if (this._courseGroups.length === 0) {
      return;
    }

    const dialogData = new SelectClassDialogData(this._courseGroups);
    const dialogResult = await this._dialog.show(SelectClassDialog, dialogData);

    if (!dialogResult || !dialogResult.hasData()) {
      return;
    }

    const schoolClassId = dialogResult.data;

    try {
      this.startLoading();
      const command = new CreateQuizFromTemplateCommand(
        template.templateId,
        schoolClassId,
        this._userId,
      );
      await firstValueFrom(this._api.send(command));
      this._snackBarService.showMessage('Quiz created from template and assigned!');
    } catch (err) {
      this.handleError(err);
    } finally {
      this.endLoading();
    }
  }

  formatDate(date: Date): string {
    if (!date) {
      return '';
    }
    return new Date(date).toLocaleDateString('en-AU', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
    });
  }
}
