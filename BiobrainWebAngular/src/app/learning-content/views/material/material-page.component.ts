import { Component, OnDestroy } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { of, Subscription } from 'rxjs';
import { filter, shareReplay, switchMap, tap } from 'rxjs/operators';
import { LevelTypeModel } from 'src/app/api/models/level-type.model';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveCourseService } from 'src/app/core/services/active-course.service';
import { ActiveSchoolService } from 'src/app/core/services/active-school.service';
import { StudentCourseGroup } from 'src/app/core/services/courses/student-course-group';
import { StudentCoursesService } from 'src/app/core/services/courses/student-courses.service';
import { AnalyticsService } from 'src/app/core/services/google-analitics-state.service';
import { StringsService } from 'src/app/share/strings.service';

import { ContentTreeService } from '../../../core/services/content/content-tree.service';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { toNonNullable } from '../../../share/helpers/to-non-nullable';
import { LearningContentProviderService } from '../../services/learning-content-provider.service';
import { ActiveSchoolClassService } from 'src/app/core/services/active-school-class.service';
import { TeacherCourseGroup } from 'src/app/core/services/courses/teacher-course-group';
import { TeacherCourse } from 'src/app/core/services/courses/teacher-course';
import { TeacherCoursesService } from 'src/app/core/services/courses/teacher-courses.service';
import { GetQuizResultForLevelOperation } from '../../operations/get-quiz-result-for-level.operation';
import { AppSettings } from 'src/app/share/values/app-settings';

@Component({
  selector: 'app-material-page',
  templateUrl: './material-page.component.html',
  styleUrls: ['./material-page.component.scss'],
})
export class MaterialPageComponent extends BaseComponent implements OnDestroy {
  public levels: LevelTypeModel[] = [];
  public selectedLevelId: string | undefined;
  public availableStudentCourses: StudentCourseGroup[] =[];
  public availableTeacherCourses: TeacherCourseGroup[] =[];
  public activeSchoolClassId: string|null = null;
  isDemoMode = false;

  subscriptions: Subscription[] = [];

  constructor(
    public strings: StringsService,
    private readonly _router: Router,
    private readonly _activatedRoute: ActivatedRoute,
    private readonly _studentCoursesService: StudentCoursesService,
    private readonly _teacherCoursesService: TeacherCoursesService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _learningContentProvider: LearningContentProviderService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _routingService: RoutingService,
    private readonly _analyticsService: AnalyticsService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _getQuizResultForLevelOperation: GetQuizResultForLevelOperation,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);

    this.subscriptions.push(
      this._router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe({
        next: () => void this._init(),
        error: e => this.error(e)
      }),
    );
  }

  public get routeCourseId(): string {
    return toNonNullable(this._activatedRoute.snapshot.paramMap.get('courseId'));
  }

  public get routeTopicId(): string | undefined {
    return this._activatedRoute.snapshot.queryParamMap.get('topicId') ?? undefined;
  }

  public get className(): string | undefined {
    // This needed only for teachers. This is why availableTeacherCourses array used
    // For Students should return null
    if(!this.activeSchoolClassId || this.activeSchoolClassId.length < 1 || this.availableTeacherCourses.length < 1) return undefined;
    var courses :TeacherCourse[] = [];
    var cn = courses.concat(...this.availableTeacherCourses.map(_ => _.courses)).find(_ => _.classId == this.activeSchoolClassId)?.className;
    return cn;
  }

  public onSelectedLevelIdChange(){
    this._analyticsService.handleNavigation(new NavigationEnd(0, "", location.href));
  }

  private get _routeLevelId(): string | undefined {
    return this._activatedRoute.snapshot.queryParamMap.get('levelId') ?? undefined;
  }

  public ngOnDestroy(): void {
    this.subscriptions.forEach(x => x.unsubscribe());
  }

  private async _init(): Promise<void> {
    this._activeCourseService.setActiveCourseId(this.routeCourseId);
    if (!this.routeTopicId) {
      let firstTopicId = (await firstValueFrom(this._contentTreeService.getFirstLevel(this.routeCourseId)))?.parent?.nodeId;
      if (firstTopicId)
        this._routingService.navigateToMaterialPage(this.routeCourseId, firstTopicId, undefined);
    }

    this.activeSchoolClassId = await this._activeSchoolClassService.schoolClassId;
    this.availableTeacherCourses = await firstValueFrom(this._currentUserService.userChanges$.pipe(
      switchMap(user =>  hasValue(user) && user.isTeacher()
      ? this._teacherCoursesService.getTeacherCourses(user.userId)
      : of([]),
    )));
    this.availableStudentCourses = await firstValueFrom(this._currentUserService.userChanges$.pipe(
      switchMap(user => hasValue(user) && user.isStudent()
        ? this._studentCoursesService.getStudentCourses(user.userId)
        : of([]),
      ),
      tap(_ => _.forEach(gr => gr.courses.forEach(x => x.course.displayName = x.className))),
      shareReplay({
        bufferSize: 1,
        refCount: true
      }),
    ));
    
    this.setSchoolIfExist();

    var user = await this._currentUserService.user;
    this.isDemoMode = user?.isDemoSubscription() ?? true;
    await this._bindLevels(this.routeTopicId);
    this._setSelectedLevel(this._routeLevelId);
  }
  
  setSchoolIfExist() {
    var selectedCoursesGroups = this.availableStudentCourses.filter(_ => _.courses.some(c => c.courseId == this.routeCourseId));
    if(selectedCoursesGroups.length == 1 && selectedCoursesGroups[0].schoolId){
      this._activeSchoolService.setSchoolId(selectedCoursesGroups[0].schoolId);
      this._activeSchoolService.setSchoolName(selectedCoursesGroups[0].schoolName);
    }
  }

  private async _bindLevels(topicId: string | undefined): Promise<void> {
    this.levels = await this._getLevels(topicId);
  }

  private async _getLevels(topicId: string | undefined): Promise<LevelTypeModel[]> {
    if (!hasValue(topicId)) return [];

    const topic = await firstValueFrom(this._contentTreeService.findNode(topicId));
    if (!hasValue(topic)) return [];

    const existingLevels = topic.children.map(_ => new LevelTypeModel(_.nodeId, _.row.name, _.row.name, _.isAvailableInDemo || !this.isDemoMode));
    let levels = [];
    for (const level of existingLevels) {
      const page = await this._learningContentProvider.getPageByNodeId(this.routeCourseId, level.levelTypeId);
      if (hasValue(page) && page.materials && page.materials.length > 0) levels.push(level);
    }

    levels = await this._setLevelsColors(levels);

    return levels;
  }

  private async _setLevelsColors(levels: LevelTypeModel[]): Promise<LevelTypeModel[]>{
    var result = await this._getQuizResultForLevelOperation.perform(this.routeCourseId, levels.map(_ => _.levelTypeId));

    if(!result.isSuccess()) return levels;

    result.data.forEach(levelResult => {
      var level = levels.find(_ => _.levelTypeId == levelResult.nodeId);
      if(level == null) return;

      if(levelResult.score < AppSettings.quizQuestionsNumber*0.4)
        level.isRed = true;
      else if(levelResult.score >= AppSettings.quizQuestionsNumber*0.4 && levelResult.score < AppSettings.quizQuestionsNumber*0.7)
        level.isYellow = true;
      else
        level.isGreen = true;
    });
    return levels;
  }

  private _setSelectedLevel(selectedLevelId: string | undefined): void {
    this.selectedLevelId = this._getSelectedLevelId(selectedLevelId);
  }

  private _getSelectedLevelId(selectedLevelId: string | undefined): string | undefined {
    if (!hasValue(this.levels)) return undefined;

    const selectedLevel = hasValue(selectedLevelId)
      ? this.levels.find(_ => _.levelTypeId === selectedLevelId) ?? this.levels[0]
      : this.levels[0];
    return selectedLevel?.levelTypeId;
  }
}
