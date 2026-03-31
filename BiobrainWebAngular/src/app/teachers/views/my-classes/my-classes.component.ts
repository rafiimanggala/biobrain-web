import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { filter, switchMap, tap } from 'rxjs/operators';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { ActiveSchoolService } from 'src/app/core/services/active-school.service';
import { TeacherCourseGroup } from 'src/app/core/services/courses/teacher-course-group';
import { TeacherCoursesService } from 'src/app/core/services/courses/teacher-courses.service';
import { Logger } from 'src/app/core/services/logger';
import { InformationDialog } from 'src/app/share/dialogs/information/information.dialog';
import { InformationDialogData } from 'src/app/share/dialogs/information/information.dialog-data';

import { CurrentUserService } from '../../../auth/services/current-user.service';
import { RoutingService } from '../../../auth/services/routing.service';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../../core/services/active-school-class.service';
import { hasValue } from '../../../share/helpers/has-value';
import { StringsService } from '../../../share/strings.service';

@Component({
  selector: 'app-my-classes',
  templateUrl: './my-classes.component.html',
  styleUrls: ['./my-classes.component.scss'],
})
export class MyClassesComponent extends BaseComponent {
  schoolClassesGroups$: Observable<TeacherCourseGroup[]>;

  constructor(
    public readonly strings: StringsService,
    public readonly currentUserService: CurrentUserService,
    private readonly logger: Logger,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _routingService: RoutingService,
    private readonly _teacherCoursesService: TeacherCoursesService,
    private readonly _dialog: Dialog,
    eventProvider: AppEventProvider
  ) {
    super(eventProvider);
    logger.log("My classes constructor");
    this.schoolClassesGroups$ = this.currentUserService.userChanges$.pipe(
      filter(hasValue),
      switchMap(user => this._teacherCoursesService.getTeacherCourses(user.userId)),
    );
    
    this.pushSubscribtions(this.schoolClassesGroups$.pipe(
      tap(_ => this.logger.log(`School Classes Groups ${JSON.stringify(_)}`)),
      filter(groups => !groups || groups.length < 1), 
      tap(() => this.onNoCourses())
      ).subscribe());
  }

  public async onNoCourses() {
    var result = await this._dialog.show(InformationDialog, new InformationDialogData(this.strings.freeTrial, this.strings.freeTrialPeriodConcludedTeacher, this.strings.ok, true))
  }

  onSchoolClassSelect(schoolClassId: string, courseId: string, courseGroup: TeacherCourseGroup): void {
    this._activeSchoolClassService.setActiveSchoolClassId(schoolClassId);
    this._activeCourseService.setActiveCourseId(courseId);
    this._activeSchoolService.setSchoolId(courseGroup.schoolId);
    this._activeSchoolService.setSchoolName(courseGroup.schoolName);
    void this._routingService.navigateToMaterialPage(courseId, undefined, undefined);
  }
}
