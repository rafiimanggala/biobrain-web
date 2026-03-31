import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, combineLatest } from 'rxjs';
import { filter, first, map, switchMap, take, tap } from 'rxjs/operators';
import { SchoolStatus } from 'src/app/api/enums/school-status.enum';
import { CreateSchoolCommand, CreateSchoolCommand_Result } from 'src/app/api/schools/create-school.command';
import { GetSchoolListItemsQuery, GetSchoolListItemsQuery_Result } from 'src/app/api/schools/get-school-list-items.query';
import { UpdateSchoolLicensesCommand } from 'src/app/api/schools/update-school-licenses.command';
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
export class CreateSchoolService extends DisposableSubscriptionService {
  createdSchool$: ReplaySubject<CreateSchoolCommand_Result>;

  private readonly _defaultTeacherLicenseNumber = 50;
  private readonly _defaultStudentLicenseNumber = 50000;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
  ) {
    super();
    const bufferSize = 1;
    this.createdSchool$ = new ReplaySubject<CreateSchoolCommand_Result>(bufferSize);
  }

  perform(): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing

    const $courses = this._api.send(new GetCoursesListQuery()).pipe(first());
    const $schools = this._api.send(new GetSchoolListItemsQuery()).pipe(first());

    this.subscriptions.push(
      combineLatest([$courses, $schools])
        .pipe(
          map(x => this._buildDialogData(x[1], x[0])),
          switchMap(d => this._dialog.observe(SchoolDialog, d)),
          map(_ => _.data),
          filter(hasValue),
          filter(x => Boolean(x?.name)),
          tap(() => this._loaderService.show()),
          switchMap(x => this._api.send(new CreateSchoolCommand(x.name ?? '', x.useAccessCodes, x.status ?? SchoolStatus.FreeTrial, x.endDate?.endOf('day').toJSON() ?? null, x.coursesIds)).pipe(
            switchMap(school => this._setLicenses(school, x)),
          )),
          tap(x => this.createdSchool$.next(x))
        )
        .subscribe(onFinish, onFinish),
    );
  }

  private _buildDialogData(schools: GetSchoolListItemsQuery_Result[], courses: GetCoursesListQuery_Result[]): SchoolDialogData {
    const dialogSettings = new SchoolDialogDataSettings(new Set<string>(schools.map(s => s.name)), courses.map(_ => {return {courseId: _.courseId, name: _.name, isSelected: false};}));
    const dialogData = new SchoolDialogData(dialogSettings);
    dialogData.studentsLicensesNumber = this._defaultStudentLicenseNumber;
    dialogData.teachersLicensesNumber = this._defaultTeacherLicenseNumber;

    return dialogData;
  }

  private _setLicenses(school: CreateSchoolCommand_Result, dialogData: SchoolDialogData): Observable<CreateSchoolCommand_Result> {
    return this._api.send(
      new UpdateSchoolLicensesCommand(
        school.schoolId,
        dialogData.teachersLicensesNumber ?? this._defaultTeacherLicenseNumber,
        dialogData.studentsLicensesNumber ?? this._defaultStudentLicenseNumber
      )
    ).pipe(map(() => school));
  }
}
