import { Injectable } from '@angular/core';
import { _ } from 'ag-grid-community';
import { ReplaySubject } from 'rxjs';
import { Api } from 'src/app/api/api.service';
import { AddExistingStudentToSchoolCommand } from 'src/app/api/students/add-existing-student-by-email.command';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriptionService } from 'src/app/core/services/disposable-subscription.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { getExceptionMessage } from 'src/app/share/helpers/get-exception-message';
import { Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { LoaderService } from 'src/app/share/services/loader.service';
import { InviteStudentDialog } from 'src/app/teachers/dialogs/invite-student/invite-student.dialog';


@Injectable({
  providedIn: 'root'
})
export class AddExistingStudentToSchoolOperation extends DisposableSubscriptionService {
  addExisting$: ReplaySubject<string>;
  constructor(
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
    private readonly _appEvents: AppEventProvider
  ) {
    super();
    const bufferSize = 1;
    this.addExisting$ = new ReplaySubject<string>(
      bufferSize
    );
  }

  async perform(schoolId: string): Promise<SuccessOrFailedResult> {
    // 
    const dialogResult = await this._dialog.show(InviteStudentDialog, {
      email: "",
    });
    if (dialogResult.action !== DialogAction.yes || !dialogResult.data?.email) return Result.failed();

    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new AddExistingStudentToSchoolCommand(dialogResult.data.email, schoolId)));
      this.addExisting$.next(dialogResult.data.email);
      return Result.success();
    } catch (e: any) {
      this._appEvents.errorEmit(getExceptionMessage(e));
      return Result.failed();
    }
    finally{
      this._loaderService.hide();
    }
  }
}
