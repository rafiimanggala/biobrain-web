import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { GetTeacherByIdQuery } from 'src/app/api/teachers/get-teacher-by-id.query';
import { UpdateTeacherDetailsCommand, UpdateTeacherDetailsCommand_Result } from 'src/app/api/teachers/update-teacher-details.command';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';

import { Api } from '../../../api/api.service';
import { TeacherDialog } from '../dialogs/teacher-dialog/teacher-dialog.component';


@Injectable()
export class UpdateTeacherDetailsService extends DisposableSubscriptionService {
  updatedTeacher$: ReplaySubject<UpdateTeacherDetailsCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.updatedTeacher$ = new ReplaySubject<UpdateTeacherDetailsCommand_Result>(bufferSize);
  }

  perform(teacherId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    // TODO: implement server error processing
    this.subscriptions.push(
      this._api
        .send(new GetTeacherByIdQuery(teacherId))
        .pipe(
          take(1),
          map(x => ({ email: x.email, firstName: x.firstName, lastName: x.lastName, isEditMode: true })),
          switchMap(x => this._dialog.observe(TeacherDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          tap(() => this._loaderService.show()),
          map(d => new UpdateTeacherDetailsCommand(teacherId, d.firstName ?? '', d.lastName ?? '')),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.updatedTeacher$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }
}
