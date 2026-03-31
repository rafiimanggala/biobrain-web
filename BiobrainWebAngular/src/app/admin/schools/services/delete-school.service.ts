import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { DeleteSchoolCommand, DeleteSchoolCommand_Result } from 'src/app/api/schools/delete-school.command';
import { GetSchoolByIdQuery, GetSchoolByIdQuery_Result } from 'src/app/api/schools/get-school-by-id.query';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';

import { Api } from '../../../api/api.service';

@Injectable()
export class DeleteSchoolService extends DisposableSubscriptionService {
  deletedSchool$: ReplaySubject<DeleteSchoolCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.deletedSchool$ = new ReplaySubject<DeleteSchoolCommand_Result>(
      bufferSize
    );
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
          map(x => this._buildDialogData(x)),
          switchMap(x => this._dialog.observe(DeleteConfirmationDialog, x)),
          map(_ => _.data),
          filter(x => x?.confirmed === true),
          tap(() => this._loaderService.show()),
          map(() => new DeleteSchoolCommand(schoolId)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.deletedSchool$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }

  private _buildDialogData(x: GetSchoolByIdQuery_Result): DeleteConfirmationDialogData {
    return new DeleteConfirmationDialogData(this._strings.school, x.name);
  }
}
