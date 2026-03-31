import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { DeleteTeacherCommand, DeleteTeacherCommand_Result } from 'src/app/api/teachers/delete-teacher.command';
import { GetTeacherByIdQuery } from 'src/app/api/teachers/get-teacher-by-id.query';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';

import { Api } from '../../../api/api.service';

@Injectable()
export class DeleteTeacherService extends DisposableSubscriptionService {
  deletedTeacher$: ReplaySubject<DeleteTeacherCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.deletedTeacher$ = new ReplaySubject<DeleteTeacherCommand_Result>(bufferSize);
  }

  perform(teacherId: string, schoolId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetTeacherByIdQuery(teacherId))
        .pipe(
          take(1),
          map(x => new DeleteConfirmationDialogData(this._strings.teacher, x.fullName)),
          switchMap(x => this._dialog.observe(DeleteConfirmationDialog, x)),
          map(_ => _.data),
          filter(x => x?.confirmed === true),
          tap(() => this._loaderService.show()),
          map(() => new DeleteTeacherCommand(teacherId, schoolId)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.deletedTeacher$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }
}
