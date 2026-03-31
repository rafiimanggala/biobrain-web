import { Injectable } from '@angular/core';
import { combineLatest } from 'rxjs';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import { GetTeacherByIdQuery } from 'src/app/api/teachers/get-teacher-by-id.query';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';

import { Api } from '../../../api/api.service';
import { GetTeacherClassesQuery } from '../../../api/teachers/get-teacher-classes.query';
import { UpdateTeacherClassesCommand } from '../../../api/teachers/update-teacher-classes.command';
import { SelectClassesDialog } from '../../dialogs/select-classes-dialog/select-classes-dialog.component';


@Injectable()
export class EditTeacherClassesOperation extends DisposableSubscriptionService {
  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService
  ) {
    super();
  }

  perform(teacherId: string, schoolId: string): void {
    const onFinish = (): void => {
      this._loaderService.hide();
    };

    const data$ = combineLatest([
      this._api.send(new GetTeacherClassesQuery(teacherId, schoolId))
    ]);

    const do$ = data$.pipe(
      map(([getClassesResult]) => {
        const schoolClassIds = getClassesResult.map(_ => _.schoolClassId);
        return { schoolId: schoolId, schoolClassIds };
      }),
      switchMap(dialogData => this._dialog.observe(SelectClassesDialog, dialogData)),
      map(_ => _.data),
      filter(hasValue),
      tap(() => this._loaderService.show()),
      map(dialogResult => new UpdateTeacherClassesCommand(teacherId, dialogResult.schoolClassIds)),
      switchMap(cmd => this._api.send(cmd))
    );

    // TODO: implement server error processing
    this.subscriptions.push(do$.subscribe(onFinish, onFinish));
  }
}
