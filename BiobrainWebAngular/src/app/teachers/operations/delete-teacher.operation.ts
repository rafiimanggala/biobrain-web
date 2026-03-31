import { Injectable } from '@angular/core';
import { DeleteTeacherFromSchoolClassCommand } from 'src/app/api/students/delete-teacher-from-school-class.command';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';

@Injectable({
  providedIn: 'root',
})
export class DeleteTeacherOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
  ) {
  }

  public async perform(classId: string, teacherId: string, teacherName: string): Promise<SuccessOrFailedResult<string, string>> {

    const dialogResult = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this.strings.removeTeacherForClass, this.strings.confirmTeacherDelete(teacherName)));

    if (dialogResult.action !== DialogAction.yes) return Result.failed("");

    try {
      await firstValueFrom(this._api.send(new DeleteTeacherFromSchoolClassCommand(teacherId, classId)));
      this._snackBarService.showMessage(this.strings.deleteTeacherSuccessMessage(teacherName));
      return Result.success(teacherId);
    } catch (e: any) {
      const msg = 'error' in e ? (e as { error: string }).error : e as string;
      this._appEvents.errorEmit(msg);
      return Result.failed("");
    }
  }
}
