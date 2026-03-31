import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, of } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { Api } from 'src/app/api/api.service';
import { GetSchoolLicenseInfoQuery } from 'src/app/api/schools/get-school-license-info.query';
import { CreateStudentCommand, CreateStudentCommand_Result } from 'src/app/api/students/create-student.command';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { InformationDialog } from 'src/app/share/dialogs/information/information.dialog';
import { InformationDialogData } from 'src/app/share/dialogs/information/information.dialog-data';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';
import { worldCountries } from 'src/app/share/values/countries';
import { isValid, StudentDialogData, StudentDialogDataSettings } from '../student-dialog/student-dialog-data';
import { StudentDialog } from '../student-dialog/student-dialog.component';


@Injectable()
export class CreateStudentService extends DisposableSubscriptionService {
  createdStudent$: ReplaySubject<CreateStudentCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
    private readonly _strings: StringsService
  ) {
    super();

    const bufferSize = 1;
    this.createdStudent$ = new ReplaySubject<CreateStudentCommand_Result>(
      bufferSize
    );
  }

  perform(schoolId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._confirmLicense(schoolId)
        .pipe(
          take(1),
          filter(isConfirmed => isConfirmed),
          map(() => ({
            // Default settings
            country: worldCountries[0].name,
            state: "Victoria",
            curriculumCode: 1,
            settings: new StudentDialogDataSettings(false)
          } as StudentDialogData)),
          switchMap(x => this._dialog.observe(StudentDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          filter(x => isValid(x)),
          tap(() => this._loaderService.show()),
          map(x => new CreateStudentCommand(schoolId, x.email ?? '', x.firstName ?? '', x.lastName ?? '', x.country ?? '', x.state ?? '', x.curriculumCode ?? -1, x.year ?? -1)),
          switchMap(x => this._api.send(x)),
          tap(x => this.createdStudent$.next(x))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  private _confirmLicense(schoolId: string): Observable<boolean> {
    return this._api.send(new GetSchoolLicenseInfoQuery(schoolId)).pipe(
      take(1),
      switchMap(licenseInfo => {
        const canCreateTeacher = licenseInfo.actualStudentsCount + 1 <= licenseInfo.studentLicensesNumber;
        if (canCreateTeacher) {
          return of(true);
        }

        const dialogData = new InformationDialogData(
          this._strings.unableToPerformOperation,
          this._strings.studentLicenseLimitExceeded(licenseInfo.studentLicensesNumber),
        );

        this.subscriptions.push(
          this._dialog.observe(InformationDialog, dialogData).pipe(
            take(1)
          ).subscribe()
        );

        return of(false);
      })
    );
  }
}
