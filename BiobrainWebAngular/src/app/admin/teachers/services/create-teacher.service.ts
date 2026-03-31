import { Injectable } from '@angular/core';
import { Observable, ReplaySubject, of } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { GetSchoolLicenseInfoQuery } from 'src/app/api/schools/get-school-license-info.query';
import { CreateTeacherCommand, CreateTeacherCommand_Result } from 'src/app/api/teachers/create-teacher.command';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { InformationDialog } from 'src/app/share/dialogs/information/information.dialog';
import { InformationDialogData } from 'src/app/share/dialogs/information/information.dialog-data';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';

import { Api } from '../../../api/api.service';
import { TeacherDialogData } from '../dialogs/teacher-dialog/teacher-dialog-data';
import { TeacherDialog } from '../dialogs/teacher-dialog/teacher-dialog.component';


@Injectable()
export class CreateTeacherService extends DisposableSubscriptionService {
  createdTeacher$: ReplaySubject<CreateTeacherCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
    private readonly _strings: StringsService
  ) {
    super();

    const bufferSize = 1;
    this.createdTeacher$ = new ReplaySubject<CreateTeacherCommand_Result>(bufferSize);
  }

  perform(schoolId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._confirmLicense(schoolId)
        .pipe(
          filter(isConfirmed => isConfirmed),
          switchMap(() => this._dialog.observe(TeacherDialog, { isEditMode: false } as TeacherDialogData)),
          map(r => r.data),
          filter(hasValue),
          tap(() => this._loaderService.show()),
          filter(x => Boolean(x.email) && Boolean(x.firstName) && Boolean(x.lastName)),
          map(x => new CreateTeacherCommand(schoolId, x.email ?? '', x.firstName ?? '', x.lastName ?? '')),
          switchMap(x => this._api.send(x)),
          tap(x => this.createdTeacher$.next(x))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  private _confirmLicense(schoolId: string): Observable<boolean> {
    return this._api.send(new GetSchoolLicenseInfoQuery(schoolId)).pipe(
      take(1),
      switchMap(licenseInfo => {
        const canCreateTeacher = licenseInfo.actualTeachersCount + 1 <= licenseInfo.teachersLicensesNumber;
        if (canCreateTeacher) {
          return of(true);
        }

        const dialogData = new InformationDialogData(
          this._strings.unableToPerformOperation,
          this._strings.teachersLicenseLimitExceeded(licenseInfo.teachersLicensesNumber)
        );

        this.subscriptions.push(
          this._dialog.observe(InformationDialog, dialogData).pipe(take(1)).subscribe()
        );

        return of(false);
      })
    );
  }
}
