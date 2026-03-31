import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { GetSchoolByIdQuery } from 'src/app/api/schools/get-school-by-id.query';
import { UpdateSchoolLicensesCommand, UpdateSchoolLicensesCommand_Result } from 'src/app/api/schools/update-school-licenses.command';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';

import { Api } from '../../../api/api.service';
import { SchoolLicensesDialogData } from '../dialogs/school-licenses-dialog/school-licenses-dialog-data';
import { SchoolLicensesDialogSettings } from '../dialogs/school-licenses-dialog/school-licenses-dialog-settings';
import { SchoolLicensesDialog } from '../dialogs/school-licenses-dialog/school-licenses-dialog.component';


@Injectable()
export class UpdateSchoolLicensesService extends DisposableSubscriptionService {
  updatedSchool$: ReplaySubject<UpdateSchoolLicensesCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.updatedSchool$ = new ReplaySubject<UpdateSchoolLicensesCommand_Result>(bufferSize);
  }

  perform(schoolId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetSchoolByIdQuery(schoolId))
        .pipe(
          take(1),
          map(x => new SchoolLicensesDialogData(
            x.teachersLicensesNumber,
            x.studentLicensesNumber,
            new SchoolLicensesDialogSettings(x.name)
          )),
          switchMap(x => this._dialog.observe(SchoolLicensesDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          tap(() => this._loaderService.show()),
          map(dd => new UpdateSchoolLicensesCommand(schoolId, dd.teachersLicensesNumber, dd.studentsLicensesNumber)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.updatedSchool$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }
}
