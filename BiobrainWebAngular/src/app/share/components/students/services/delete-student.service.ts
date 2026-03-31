import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { Api } from 'src/app/api/api.service';
import { DeleteStudentCommand, DeleteStudentCommand_Result } from 'src/app/api/students/delete-student.command';
import { GetStudentByIdQuery } from 'src/app/api/students/get-student-by-id.query';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';


@Injectable()
export class DeleteStudentService extends DisposableSubscriptionService {
  deletedStudent$: ReplaySubject<DeleteStudentCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.deletedStudent$ = new ReplaySubject<DeleteStudentCommand_Result>(
      bufferSize
    );
  }

  perform(studentId: string, schoolId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetStudentByIdQuery(studentId))
        .pipe(
          take(1),
          map(x => new DeleteConfirmationDialogData(this._strings.student, x.fullName)),
          switchMap(x => this._dialog.observe(DeleteConfirmationDialog, x)),
          map(_ => _.data),
          filter(x => x?.confirmed === true),
          tap(() => this._loaderService.show()),
          map(() => new DeleteStudentCommand(studentId, schoolId)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.deletedStudent$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }
}
