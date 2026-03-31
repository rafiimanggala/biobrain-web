import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { filter, map, switchMap, take, tap } from 'rxjs/operators';
import { GetSchoolClassByIdQuery } from 'src/app/api/school-classes/get-school-class-by-id.query';
import { UpdateSchoolClassCommand, UpdateSchoolClassCommand_Result } from 'src/app/api/school-classes/update-school-class.command';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { LoaderService } from 'src/app/share/services/loader.service';

import { Api } from '../../../api/api.service';
import { SchoolClassDialogSettings, isValid } from '../dialogs/school-class-dialog/school-class-dialog-data';
import { SchoolClassDialog } from '../dialogs/school-class-dialog/school-class-dialog.component';

@Injectable()
export class UpdateSchoolClassService extends DisposableSubscriptionService {
  updateSchoolClass$: ReplaySubject<UpdateSchoolClassCommand_Result>;

  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService
  ) {
    super();

    const bufferSize = 1;
    this.updateSchoolClass$ = new ReplaySubject<UpdateSchoolClassCommand_Result>(bufferSize);
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
          map(x => ({
            name: x.name,
            year: x.year,
            courseId: x.courseId,
            teacherIds: x.teacherIds,
            settings: new SchoolClassDialogSettings(x.schoolId, x.schoolClassId)
          })),
          switchMap(x => this._dialog.observe(SchoolClassDialog, x)),
          map(_ => _.data),
          filter(hasValue),
          filter(dialogData => isValid(dialogData)),
          tap(() => this._loaderService.show()),
          map(dd => new UpdateSchoolClassCommand(schoolClassId, dd.year ?? 0, dd.name ?? '', dd.teacherIds)),
          switchMap(cmd => this._api.send(cmd)),
          tap(result => this.updateSchoolClass$.next(result))
        )
        .subscribe(onFinish, onFinish)
    );
  }
}
