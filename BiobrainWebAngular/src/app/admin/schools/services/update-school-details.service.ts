import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, combineLatest } from 'rxjs';
import { filter, first, map, switchMap, take, tap, withLatestFrom } from 'rxjs/operators';
import { SchoolStatus } from 'src/app/api/enums/school-status.enum';
import { GetSchoolByIdQuery, GetSchoolByIdQuery_Result } from 'src/app/api/schools/get-school-by-id.query';
import {
  GetSchoolListItemsQuery,
  GetSchoolListItemsQuery_Result
} from 'src/app/api/schools/get-school-list-items.query';
import {
  UpdateSchoolDetailsCommand,
  UpdateSchoolDetailsCommand_Result
} from 'src/app/api/schools/update-school-details.command';
import {
  UpdateSchoolLicensesCommand,
  UpdateSchoolLicensesCommand_Result
} from 'src/app/api/schools/update-school-licenses.command';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';

import { Api } from '../../../api/api.service';
import { SchoolDialogData } from '../dialogs/school-dialog/school-dialog-data';
import { SchoolDialogDataSettings } from '../dialogs/school-dialog/school-dialog-data-settings';
import { SchoolDialog } from '../dialogs/school-dialog/school-dialog.component';
import { GetCoursesListQuery, GetCoursesListQuery_Result } from 'src/app/api/content/get-courses-list.query';


@Injectable()
export class UpdateSchoolDetailsService extends DisposableSubscriptionService {
  updatedSchool$: ReplaySubject<UpdateSchoolDetailsCommand_Result>;

  private readonly _defaultTeacherLicenseNumber = 50;
  private readonly _defaultStudentLicenseNumber = 50000;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
  ) {
    super();
    const bufferSize = 1;
    this.updatedSchool$ = new ReplaySubject<UpdateSchoolDetailsCommand_Result>(bufferSize);
  }

  perform(schoolId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    var $courses = this._api.send(new GetCoursesListQuery()).pipe(first());
    var $schools = this._api.send(new GetSchoolListItemsQuery()).pipe(first());
    var $school = this._api.send(new GetSchoolByIdQuery(schoolId)).pipe(first());
    // TODO: implement server error processing
    this.subscriptions.push(
      combineLatest([$school, $schools, $courses])
        .pipe(
          map(([school, schools, courses]) => this._buildDialogData(school, schools, courses)),
          switchMap(x => this._dialog.observe(SchoolDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          filter(dd => Boolean(dd.name)),
          tap(() => this._loaderService.show()),
          switchMap(dd =>
            this._setDetails(schoolId, dd).pipe(
              switchMap(result => this._setLicenses(schoolId, dd).pipe(map(() => result)))
            )
          ),
          tap(result => this.updatedSchool$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  private _buildDialogData(
    school: GetSchoolByIdQuery_Result,
    schools: GetSchoolListItemsQuery_Result[],
    courses: GetCoursesListQuery_Result[],
  ): SchoolDialogData {
    const dialogSettings = new SchoolDialogDataSettings(
      new Set<string>(schools.filter(s => s.schoolId !== school.schoolId).map(s => s.name)),
      courses.map(_ => {return {courseId: _.courseId, name: _.name, isSelected: school.courses.some(c => _.courseId == c)};})
    );

    const dialogData = new SchoolDialogData(dialogSettings);
    dialogData.name = school.name;
    dialogData.status = school.status;
    dialogData.teachersLicensesNumber = school.teachersLicensesNumber;
    dialogData.studentsLicensesNumber = school.studentLicensesNumber;
    dialogData.endDate = school.endDate;
    dialogData.useAccessCodes = school.useAccessCodes;

    return dialogData;
  }

  private _setDetails(schoolId: string, dialogData: SchoolDialogData): Observable<UpdateSchoolDetailsCommand_Result> {
    return this._api.send(new UpdateSchoolDetailsCommand(schoolId, dialogData.name ?? '', dialogData.useAccessCodes, dialogData.status ?? SchoolStatus.FreeTrial, dialogData.endDate?.endOf('day').toJSON() ?? null, dialogData.coursesIds));
  }

  private _setLicenses(schoolId: string, dialogData: SchoolDialogData): Observable<UpdateSchoolLicensesCommand_Result> {
    return this._api.send(
      new UpdateSchoolLicensesCommand(
        schoolId,
        dialogData.teachersLicensesNumber ?? this._defaultTeacherLicenseNumber,
        dialogData.studentsLicensesNumber ?? this._defaultStudentLicenseNumber
      )
    );
  }
}
