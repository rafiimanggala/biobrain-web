import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { DeleteSchoolClassCommand, DeleteSchoolClassCommand_Result } from 'src/app/api/school-classes/delete-school-class.command';
import { GetSchoolClassByIdQuery } from 'src/app/api/school-classes/get-school-class-by-id.query';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';

import { Api } from '../../../api/api.service';

@Injectable()
export class DeleteSchoolClassService extends DisposableSubscriptionService {
  deletedSchoolClass$: ReplaySubject<DeleteSchoolClassCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.deletedSchoolClass$ = new ReplaySubject<DeleteSchoolClassCommand_Result>(
      bufferSize
    );
  }

  perform(schoolClassId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetSchoolClassByIdQuery(schoolClassId))
        .pipe(
          take(1),
          switchMap(x => this._dialog.observe(
            DeleteConfirmationDialog,
            new DeleteConfirmationDialogData(this._strings.class, x.name)
          )),
          map(_ => _.data),
          filter(x => x?.confirmed === true),
          tap(() => this._loaderService.show()),
          map(() => new DeleteSchoolClassCommand(schoolClassId)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.deletedSchoolClass$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }
}
