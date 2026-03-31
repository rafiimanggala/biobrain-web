import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable, of } from 'rxjs';
import { filter, switchMap, tap } from 'rxjs/operators';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { ActiveSchoolService } from 'src/app/core/services/active-school.service';
import { StudentCourseGroup } from 'src/app/core/services/courses/student-course-group';
import { StudentProfileService } from 'src/app/core/services/students/student-profile.service';
import { ActionListDialog } from 'src/app/share/dialogs/action-list/action-list.dialog';
import { Action, ActionListDialogData } from 'src/app/share/dialogs/action-list/action-list.dialog-data';

import { CurrentUserService } from '../../../auth/services/current-user.service';
import { RoutingService } from '../../../auth/services/routing.service';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { StudentCourse } from '../../../core/services/courses/student-course';
import { StudentCoursesService } from '../../../core/services/courses/student-courses.service';
import { hasValue } from '../../../share/helpers/has-value';
import { StringsService } from '../../../share/strings.service';
import { UseAccessCodeOperation } from '../../operations/use-access-code.operation';

@Component({
  selector: 'app-my-courses',
  templateUrl: './my-courses.component.html',
  styleUrls: ['./my-courses.component.scss'],
})
export class MyCoursesComponent extends BaseComponent {
  public readonly studentCourseGroups$: Observable<StudentCourseGroup[]>;
  public readonly signup: boolean;
  public readonly purchaseDialogActions: Action[] = [
    { text: this.strings.cancel, code: 0 },
    { text: this.strings.useAccessCode, code: 1 },
    { text: this.strings.purchase, code: 2 }
  ];

  constructor(
    public readonly strings: StringsService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _routingService: RoutingService,
    private readonly _studentCoursesStore: StudentCoursesService,
    private readonly _studentProfileService: StudentProfileService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _useAccessCodeOperation: UseAccessCodeOperation,
    private readonly _dialog: Dialog,
    public readonly currentUserService: CurrentUserService,
    _activatedRouteSnapshot: ActivatedRoute,
    appEvents: AppEventProvider) {
    super(appEvents);
    this.studentCourseGroups$ = this.currentUserService.userChanges$.pipe(
      switchMap(user => hasValue(user) ? this._studentCoursesStore.getStudentCourses(user.userId) : of([])),
    );
    this.pushSubscribtions(
      this.studentCourseGroups$.pipe(
        filter(courses => courses.length < 1),
        tap(_ => this.onNoCourses())
      ).subscribe());
    this.signup = Boolean(JSON.parse(_activatedRouteSnapshot.snapshot.queryParamMap.get('signup') ?? 'false'));
  }

  public async onNoCourses() {
    var user = await this.currentUserService.user;
    if (!user) return;
    var state = await this._studentProfileService.getStudentProfileState(user.userId);

    if (state.isFromArchivedSchool) {
      var result = await this._dialog.show(ActionListDialog, new ActionListDialogData(this.strings.freeTrial, this.strings.freeTrialPeriodConcluded, this.purchaseDialogActions))
      if (result.action != DialogAction.yes) return;
      if (!result.data) return;
      await this.handleNoCourseDialogResult(result.data);
    }
    else {
      var result = await this._dialog.show(ActionListDialog, new ActionListDialogData(this.strings.noCoursesHeader, this.strings.noCoursesMessage, this.purchaseDialogActions))
      if (result.action != DialogAction.yes) return;
      if (!result.data) return;
      await this.handleNoCourseDialogResult(result.data);
    }
  }

  public async handleNoCourseDialogResult(action: Action) {
    switch (action.code) {
      // AccessCode
      case 1:
        await this._useAccessCodeOperation.perform();
        return;

      // Purchase
      case 2:
        await this._routingService.navigateToSubscriptionPage();
        return;

      // Cancel
      case 0:
      default:
        return;
    }
  }

  public onCourseSelected(studentCourse: StudentCourse, courseGroup: StudentCourseGroup): void {
    this._activeCourseService.setActiveCourseId(studentCourse.courseId);
    this._activeSchoolService.setSchoolId(courseGroup.schoolId);
    this._activeSchoolService.setSchoolName(courseGroup.schoolName);
    void this._routingService.navigateToMaterialPage(studentCourse.courseId, undefined, undefined);
  }
}
