import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { combineLatest, Observable, of } from 'rxjs';
import { filter, map, shareReplay, startWith, switchMap, tap } from 'rxjs/operators';
import { LogoutOperation } from 'src/app/auth/operations/logout.operation';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { NavigationItem } from 'src/app/core/models/navigation-item';

import { ChangeSelfPasswordOperation } from '../../../auth/operations/change-self-password.operation';
import { ChangeSelfEmailOperation } from '../../../auth/operations/change-self-email.operation';
import { CurrentUser } from '../../../auth/services/current-user';
import { CurrentUserService } from '../../../auth/services/current-user.service';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../../core/services/active-school-class.service';
import { Course } from '../../../core/services/courses/course';
import { StudentCoursesService } from '../../../core/services/courses/student-courses.service';
import { SidenavService } from '../../../core/services/side-nav.service';
import { JoinToClassOperation } from '../../../student/operations/join-to-class.operation';
import { hasValue } from '../../helpers/has-value';
import { EditUserProfileOperation } from '../../operations/edit-user-profile.operation';
import { StringsService } from '../../strings.service';
import { NavigationType } from '../../values/navigation-type';
import { SubscriptionsListOperation } from 'src/app/payment/operations/edit-subscription.operation';
import { StudentCourseGroup } from 'src/app/core/services/courses/student-course-group';
import { SelectSchoolOperation } from '../../operations/select-school.operation';
import { CoursesService } from 'src/app/core/services/courses/courses.service';
import { UseAccessCodeOperation } from 'src/app/student/operations/use-access-code.operation';
import { SendLogsOperation } from '../../operations/send-logs.operation';
import { ActiveSchoolService } from 'src/app/core/services/active-school.service';
import { StudentCourse } from 'src/app/core/services/courses/student-course';

@Component({
  selector: 'app-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss'],
})
export class ToolbarComponent extends BaseComponent {
  public courses$: Observable<StudentCourseGroup[]>;
  public selectedCourse$: Observable<Course | null | undefined>;

  public menuItems$: Observable<NavigationItem[]>;
  public mySubjectsVisible$: Observable<boolean>;
  public myClassesVisible$: Observable<boolean>;
  public canJoinToClass$: Observable<boolean>;
  public canUseAccessCode$: Observable<boolean>;
  public canEditProfile$: Observable<boolean>;
  public canChangePassword$: Observable<boolean>;
  public canChangeEmail$: Observable<boolean>;
  public canSendLogs$ : Observable<boolean>;
  public myAccountVisible$: Observable<boolean>;
  public navigationType$: Observable<NavigationType>;
  public canSeeUserGuides$: Observable<boolean>;
  public canManageSchool$: Observable<boolean>;

  public selectedTab = '';

  constructor(
    public strings: StringsService,
    public readonly routingService: RoutingService,
    public readonly currentUserService: CurrentUserService,
    public readonly sidenavService: SidenavService,
    private readonly _logoutOperation: LogoutOperation,
    private readonly _changeSelfPasswordOperation: ChangeSelfPasswordOperation,
    private readonly _joinToClassOperation: JoinToClassOperation,
    private readonly _useAccessCodeOperation: UseAccessCodeOperation,
    private readonly _editUserProfileOperation: EditUserProfileOperation,
    private readonly _changeSelfEmailOperation: ChangeSelfEmailOperation,
    private readonly _editSubscriptionOperation: SubscriptionsListOperation,
    private readonly _selectSchoolOperation: SelectSchoolOperation,
    private readonly _sendLogsOperation: SendLogsOperation,
    private readonly _studentCoursesService: StudentCoursesService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _coursesService: CoursesService,
    private readonly _router: Router,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);
    this.menuItems$ = this.currentUserService.userChanges$.pipe(
      switchMap(user => this._getNavigationItems(user)),
    );

    this.navigationType$ = this.currentUserService.userChanges$.pipe(
      switchMap(user => this._getNavigationType(user)),
    );

    this.courses$ = this.currentUserService.userChanges$.pipe(
      switchMap(user => hasValue(user) && user.isStudent()
        ? this._studentCoursesService.getStudentCourses(user.userId)
        : of([]),
      ),
      tap(_ => _.forEach(gr => gr.courses.forEach(x => x.course.displayName = x.className))),
      shareReplay({
        bufferSize: 1,
        refCount: true
      }),
    );

    this.mySubjectsVisible$ = this.courses$.pipe(
      map(courses => courses.length > 0),
      startWith(true),
    );

    this.myClassesVisible$ = combineLatest([this.currentUserService.userChanges$, this._activeCourseService.courseIdChanges$]).pipe(
      map(([user, courseId]) => hasValue(courseId) && hasValue(user) && user.isTeacher())
    );

    this.selectedCourse$ = this._activeCourseService.courseChanges$;

    this.canJoinToClass$ = this.currentUserService.userChanges$.pipe(
      switchMap(_ => _joinToClassOperation.canPerform()),
      map(_ => _.isSuccess()),
    );

    this.canSeeUserGuides$ = this.currentUserService.userChanges$.pipe(
      map(_ => (_?.isTeacher() ?? false) || (_?.isLiveStudent() ?? false))
    );

    this.canUseAccessCode$ = this.currentUserService.userChanges$.pipe(
      switchMap(_ => _useAccessCodeOperation.canPerform()),
      map(_ => _.isSuccess()),
    );

    this.canChangePassword$ = this.currentUserService.userChanges$.pipe(
      switchMap(_ => _changeSelfPasswordOperation.canPerform()),
      map(_ => _.isSuccess()),
    );

    this.canEditProfile$ = this.currentUserService.userChanges$.pipe(
      switchMap(_ => _editUserProfileOperation.canPerform()),
      map(_ => _.isSuccess()),
    );

    this.canSendLogs$ = this.currentUserService.userChanges$.pipe(
      switchMap(_ => _sendLogsOperation.canPerform()),
      map(_ => _.isSuccess()),
    );

    this.canChangeEmail$ = this.currentUserService.userChanges$.pipe(
      switchMap(_ => _changeSelfEmailOperation.canPerform()),
      map(_ => _.isSuccess()),
    );

    this.myAccountVisible$ = this.currentUserService.userChanges$.pipe(
      map(_ => _?.isStudent() ?? false),
    );

    this.canManageSchool$ = this.currentUserService.userChanges$.pipe(
      map(user => { return user?.isSchoolAdmin() ?? false }),
    );

    // this.subscriptions.push(this._router.events.subscribe((event) => {
    //   if (event instanceof NavigationEnd) this.selectedTab = ;
    // }));
    this.subscriptions.push(
      this._router.events.pipe(
        filter(x => x instanceof NavigationEnd),
        switchMap(event => this.menuItems$.pipe(
          map(menuItems => {
            const navEnd = <NavigationEnd>event;
            this.selectedTab = menuItems.find(x => navEnd.url.includes(x.navigation))?.navigation ?? '';
          })
        ))
      ).subscribe());
  }

  public toggleMenu(): void {
    this.sidenavService.toggle();
  }

  public async homeClick(): Promise<void> {
    await this.routingService.navigateToHome();
  }

  public async logout(): Promise<void> {
    await this._logoutOperation.perform();
  }

  public onCourseSelected($event: StudentCourse, schoolId: string, schoolName:string): void {
    this._activeCourseService.setActiveCourseId($event.courseId);
    this._activeSchoolClassService.setActiveSchoolClassId($event.classId);
    this._activeSchoolService.setSchoolId(schoolId);
    this._activeSchoolService.setSchoolName(schoolName);
    void this.routingService.navigateToMaterialPage($event.courseId, undefined, undefined);
  }

  public async onManageSchool() {
    var user = await this.currentUserService.user;
    if (!hasValue(user) || !user.adminSchoolIds) return;

    if (user.adminSchoolIds.length == 1) {
      this.routingService.navigateToTeachersAdminPage(user.adminSchoolIds[0]);
      return;
    }

    var result = await this._selectSchoolOperation.perform(user.adminSchoolIds);
    if (!result.isSuccess()) return;

    this.routingService.navigateToTeachersAdminPage(result.data);
  }

  public async onJoinToClass(): Promise<void> {
    await this._joinToClassOperation.perform();
  }

  public async onUseAccessCode(): Promise<void> {
    await this._useAccessCodeOperation.perform();
  }

  public async onChangePassword(): Promise<void> {
    await this._changeSelfPasswordOperation.perform();
  }

  public async onSendLogs(): Promise<void> {
    await this._sendLogsOperation.perform();
  }

  public async onEditProfile(): Promise<void> {
    await this._editUserProfileOperation.perform();
  }

  public async onChangeEmail(): Promise<void> {
    await this._changeSelfEmailOperation.perform();
  }

  public async onMyAccountClick(): Promise<void> {
    await this._editSubscriptionOperation.perform();
  }

  public async onUserGuidesClick(): Promise<void>{
    await this.routingService.navigateToUserGuides();
  }

  private _getNavigationItems(user: CurrentUser | undefined): Observable<NavigationItem[]> {
    if (!hasValue(user)) return of([]);
    if (user.isStudent()) return this._getStudentNavigation();
    if (user.isTeacher()) return this._getTeacherNavigation();
    return of([]);
  }

  private _getNavigationType(user: CurrentUser | undefined): Observable<NavigationType> {
    if (!hasValue(user)) return of(NavigationType.Default);
    if (user.isStudent()) return of(NavigationType.Default);
    if (user.isTeacher()) return of(NavigationType.Tabs);
    return of(NavigationType.Default);
  }

  private _getStudentNavigation(): Observable<NavigationItem[]> {
    return this._activeCourseService.courseIdChanges$.pipe(
      switchMap(courseId => {
        if (!hasValue(courseId)) {
          return [];
        }
        return this._coursesService.findById(courseId);

      }),
      map(course => {
        if (!hasValue(course)) {
          return [];
        }
        var items = [
          new NavigationItem(this.strings.glossary.toUpperCase(), this.routingService.glossary().toString()),
          new NavigationItem(this.strings.customQuiz.toUpperCase(), this.routingService.customQuiz().toString()),
          new NavigationItem(this.strings.results.toUpperCase(), this.routingService.quizResultHistory().toString()),
          new NavigationItem(this.strings.savedItems.toUpperCase(), this.routingService.bookmarks().toString()),
        ];
        // for chemistry
        if (course.subjectCode == 2 || course.subjectCode == 6)
          items.splice(1, 0, new NavigationItem(this.strings.periodicTable.toUpperCase(), this.routingService.periodicTable().toString()));
        return items;
      })
    );
  }

  private _getTeacherNavigation(): Observable<NavigationItem[]> {
    return this._activeCourseService.courseIdChanges$.pipe(
      switchMap((courseId) => {
        if (!hasValue(courseId)) {
          return [];
        }
        return combineLatest([this._coursesService.findById(courseId), this._activeSchoolClassService.schoolClassIdChanges$]);

      }),
      map(([course, schoolClassId]) => {
        if (!hasValue(course)) {
          return [];
        }
        const navigationItems = [new NavigationItem(this.strings.myClasses.toUpperCase(), this.routingService.myClasses().toString())];

        if (hasValue(course.courseId) && hasValue(schoolClassId)) {
          navigationItems.push(
            new NavigationItem(this.strings.teach.toUpperCase(), this.routingService.learningMaterials(course.courseId).toString()),
            new NavigationItem(this.strings.glossary.toUpperCase(), this.routingService.glossary().toString()),
            // new NavigationItem(this.strings.savedItems.toUpperCase(), this.routingService.bookmarks().toString()),
            new NavigationItem(this.strings.classResults.toUpperCase(), this.routingService.classResults().toString()),
            new NavigationItem(this.strings.classAdmin.toUpperCase(), this.routingService.classAdmin().toString()),
            new NavigationItem(this.strings.workAssigned.toUpperCase(), this.routingService.workAssigned().toString()),
            new NavigationItem(this.strings.createQuiz.toUpperCase(), this.routingService.teacherCustomQuiz().toString()),
            new NavigationItem(this.strings.quizTemplates.toUpperCase(), this.routingService.quizTemplates().toString()),
            new NavigationItem(this.strings.aiPracticeSet.toUpperCase(), this.routingService.aiPracticeSet().toString()),
            new NavigationItem(this.strings.aiInsights.toUpperCase(), this.routingService.aiInsights().toString()),
          );
        }

        // for chemistry
        if (course.subjectCode == 2 || course.subjectCode == 6)
          navigationItems.splice(2, 0, new NavigationItem(this.strings.periodicTable.toUpperCase(), this.routingService.periodicTable().toString()));

        return navigationItems;
      }),
    );
  }
}
