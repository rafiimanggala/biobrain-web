import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Observable } from 'rxjs';
import { distinctUntilChanged, filter, switchMap, tap } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import {
  PreviewInsightsCommand,
  PreviewInsightsCommand_Result,
} from '../../../api/ai/preview-insights.command';
import { CurrentUserService } from '../../../auth/services/current-user.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { BaseComponent } from '../../../core/app/base.component';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../../core/services/active-school-class.service';
import { TeacherCourseGroup } from '../../../core/services/courses/teacher-course-group';
import { TeacherCoursesService } from '../../../core/services/courses/teacher-courses.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-ai-insights',
  templateUrl: './ai-insights.component.html',
  styleUrls: ['./ai-insights.component.scss'],
})
export class AiInsightsComponent extends BaseComponent implements OnInit {
  courseGroups$: Observable<TeacherCourseGroup[]>;

  selectedCourseId = '';
  selectedClassId = '';
  isGenerating = false;
  insightsHtml: SafeHtml | null = null;
  hasGenerated = false;

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _sanitizer: DomSanitizer,
    private readonly _currentUserService: CurrentUserService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _activeClassService: ActiveSchoolClassService,
    private readonly _teacherCoursesService: TeacherCoursesService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);

    this.courseGroups$ = this._currentUserService.userChanges$.pipe(
      filter(hasValue),
      switchMap(user => this._teacherCoursesService.getTeacherCourses(user.userId)),
    );
  }

  ngOnInit(): void {
    this.pushSubscribtions(
      this._activeCourseService.courseIdChanges$.pipe(
        filter(hasValue),
        distinctUntilChanged(),
        tap(courseId => {
          if (this.selectedCourseId !== courseId) {
            this.selectedCourseId = courseId;
            this._resetInsights();
          }
        }),
      ).subscribe(),

      this._activeClassService.schoolClassIdChanges$.pipe(
        filter(hasValue),
        distinctUntilChanged(),
        tap(classId => {
          if (this.selectedClassId !== classId) {
            this.selectedClassId = classId;
            this._resetInsights();
          }
        }),
      ).subscribe(),
    );
  }

  onCourseChange(courseId: string): void {
    this.selectedCourseId = courseId;
    this._resetInsights();
  }

  onClassChange(classId: string): void {
    this.selectedClassId = classId;
    this._resetInsights();
  }

  get isFormValid(): boolean {
    return this.selectedCourseId.length > 0
      && this.selectedClassId.length > 0;
  }

  async onGenerateInsights(): Promise<void> {
    if (!this.isFormValid || this.isGenerating) {
      return;
    }

    this.isGenerating = true;
    this.insightsHtml = null;
    this.hasGenerated = false;

    try {
      const command = new PreviewInsightsCommand(
        this.selectedClassId,
        this.selectedCourseId,
      );

      const result: PreviewInsightsCommand_Result = await firstValueFrom(
        this._api.send(command)
      );

      this.insightsHtml = this._sanitizer.bypassSecurityTrustHtml(result.insights);
      this.hasGenerated = true;
    } catch (err) {
      this.handleError(err);
    } finally {
      this.isGenerating = false;
    }
  }

  private _resetInsights(): void {
    this.insightsHtml = null;
    this.hasGenerated = false;
  }
}
