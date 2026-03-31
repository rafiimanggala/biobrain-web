import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { Api } from 'src/app/api/api.service';
import { GetStudentByIdQuery } from 'src/app/api/students/get-student-by-id.query';
import { UpdateStudentCommand, UpdateStudentCommand_Result } from 'src/app/api/students/update-student.command';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';
import { isValid, StudentDialogData, StudentDialogDataSettings } from '../student-dialog/student-dialog-data';
import { StudentDialog } from '../student-dialog/student-dialog.component';

@Injectable()
export class UpdateStudentService extends DisposableSubscriptionService {
  updatedStudent$: ReplaySubject<UpdateStudentCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.updatedStudent$ = new ReplaySubject<UpdateStudentCommand_Result>(bufferSize);
  }

  perform(studentId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetStudentByIdQuery(studentId))
        .pipe(
          take(1),
          map(x => ({
            email: x.email,
            firstName: x.firstName,
            lastName: x.lastName,
            country: x.country,
            state: x.state,
            curriculumCode: x.curriculumCode,
            year: x.year,
            settings: new StudentDialogDataSettings(true) // unable to change class in edit mode
          } as StudentDialogData)),
          switchMap(x => this._dialog.observe(StudentDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          filter(dialogData => isValid(dialogData)),
          tap(() => this._loaderService.show()),
          map(dd => new UpdateStudentCommand(studentId, dd.firstName ?? '', dd.lastName ?? '', dd.country, dd.state, dd.curriculumCode)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.updatedStudent$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }
}
