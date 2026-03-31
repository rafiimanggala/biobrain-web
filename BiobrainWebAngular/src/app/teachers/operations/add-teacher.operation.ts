import { Injectable } from '@angular/core';
import { AddTeacherToSchoolClassCommand } from 'src/app/api/students/add-teacher-to-school-class.command';
import { GetTeacherListItemsQuery_Result } from 'src/app/api/teachers/get-teacher-list-items.query';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { AddTeacherDialog } from '../dialogs/add-teacher/add-teacher.dialog';

@Injectable({
  providedIn: 'root',
})
export class AddTeacherOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
  ) {
  }

  public async perform(classId: string, teachers: GetTeacherListItemsQuery_Result[]): Promise<SuccessOrFailedResult<string>> {
    
    if (teachers.length < 1){
      this._appEvents.errorEmit(this.strings.errors.noTeachersToSelect);
      return Result.failed();
    }

    const dialogResult = await this._dialog.show(AddTeacherDialog, {
      teachers: teachers,
      selectedTeacher: null
    });

    if (dialogResult.action !== DialogAction.yes || !dialogResult.data?.selectedTeacher) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new AddTeacherToSchoolClassCommand(dialogResult.data.selectedTeacher.teacherId, classId)));
      this._snackBarService.showMessage(this.strings.addTeacherSuccessMessage(dialogResult.data.selectedTeacher.fullName));
      return Result.success(dialogResult.data.selectedTeacher.teacherId);
    } catch (e: any) {
      const msg = 'error' in e ? (e as { error: string }).error : e as string;
      this._appEvents.errorEmit(msg);
      return Result.failed();
    }
  }
}
