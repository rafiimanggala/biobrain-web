import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { GetSchoolByIdQuery, GetSchoolByIdQuery_Result } from 'src/app/api/schools/get-school-by-id.query';
import {
  UpdateSchoolAdminsCommand,
  UpdateSchoolAdminsCommand_Result,
} from 'src/app/api/schools/update-school-admins.command';
import { GuidCollectionUpdateModel } from 'src/app/core/api/models/guid-collection-update.model';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';

import { Api } from '../../../api/api.service';
import { SchoolAdminsDialogData } from '../dialogs/school-admins-dialog/school-admins-dialog-data';
import { SchoolAdminsDialogSettings } from '../dialogs/school-admins-dialog/school-admins-dialog-settings';
import { SchoolAdminsDialog } from '../dialogs/school-admins-dialog/school-admins-dialog.component';

@Injectable()
export class UpdateSchoolAdminsService extends DisposableSubscriptionService {
  updatedSchool$: ReplaySubject<UpdateSchoolAdminsCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.updatedSchool$ = new ReplaySubject<UpdateSchoolAdminsCommand_Result>(bufferSize);
  }

  perform(schoolId: string): void {
    let existingAdmins: string[] = [];

    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetSchoolByIdQuery(schoolId))
        .pipe(
          take(1),
          tap(x => existingAdmins = x.admins ?? []),
          switchMap(x => this._dialog.observe(SchoolAdminsDialog, this._buildDialogData(x))),
          map(_ => _.data),
          filter(hasValue),
          tap(() => this._loaderService.show()),
          map(
            x =>
              new UpdateSchoolAdminsCommand(
                schoolId,
                GuidCollectionUpdateModel.asDifferenceOf(x.teachers ?? [], existingAdmins)
              )
          ),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.updatedSchool$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  private _buildDialogData(x: GetSchoolByIdQuery_Result): SchoolAdminsDialogData {
    return new SchoolAdminsDialogData(x.admins, new SchoolAdminsDialogSettings(x.name, x.schoolId));
  }
}
