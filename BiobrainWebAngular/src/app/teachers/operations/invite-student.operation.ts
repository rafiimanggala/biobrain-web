import { Injectable } from '@angular/core';
import { InviteStudentByEmailCommand } from 'src/app/api/students/invite-student-by-email.command';
import { SchoolClassModel } from 'src/app/core/services/school-class/school-class.service';
import { getExceptionMessage } from 'src/app/share/helpers/get-exception-message';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { InviteStudentDialog } from '../dialogs/invite-student/invite-student.dialog';

@Injectable({
  providedIn: 'root',
})
export class InviteStudentOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
  ) {
  }

  public async perform(schoolClass: SchoolClassModel): Promise<SuccessOrFailedResult<boolean>> {

    const dialogResult = await this._dialog.show(InviteStudentDialog, {
      email: "",
    });
    if (dialogResult.action !== DialogAction.yes || !dialogResult.data?.email) return Result.failed();

    if (schoolClass.students.some(x => x.email === dialogResult.data?.email)) {
      this._appEvents.errorEmit(this.strings.errors.studentAllReadyAdded);
      return Result.failed();
    }

    try {
      const result = await firstValueFrom(this._api.send(new InviteStudentByEmailCommand(dialogResult.data.email, schoolClass.schoolClassId)));
      this._snackBarService.showMessage(this.strings.inviteStudentSuccessMessage(dialogResult.data.email));
      return Result.success(result.isStudentAddedToClass);
    } catch (e: any) {
      this._appEvents.errorEmit(getExceptionMessage(e));
      return Result.failed();
    }
  }
}
