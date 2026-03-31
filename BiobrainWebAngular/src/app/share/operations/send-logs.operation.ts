import { Injectable } from '@angular/core';
import { SaveUserLogCommand } from 'src/app/api/accounts/save-user-log.command';
import { Logger } from 'src/app/core/services/logger';

import { Api } from '../../api/api.service';
import { GetStudentByIdQuery } from '../../api/students/get-student-by-id.query';
import { UpdateStudentCommand } from '../../api/students/update-student.command';
import { GetTeacherByIdQuery } from '../../api/teachers/get-teacher-by-id.query';
import { UpdateTeacherDetailsCommand } from '../../api/teachers/update-teacher-details.command';
import { RefreshAuthorizationOperation } from '../../auth/operations/refresh-authorization.operation';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { InformationDialog } from '../dialogs/information/information.dialog';
import { InformationDialogData } from '../dialogs/information/information.dialog-data';
import { UserProfileDialogData } from '../dialogs/user-profile-dialog/user-profile-dialog-data';
import { UserProfileDialogComponent } from '../dialogs/user-profile-dialog/user-profile-dialog.component';
import { firstValueFrom } from '../helpers/first-value-from';
import { hasValue } from '../helpers/has-value';
import { Result, SuccessOrFailedResult } from '../helpers/result';
import { LoaderService } from '../services/loader.service';
import { SnackBarService } from '../services/snack-bar.service';
import { StringsService } from '../strings.service';

@Injectable({
  providedIn: 'root',
})
export class SendLogsOperation {
  constructor(
    private readonly _strings: StringsService,
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _snackBarService: SnackBarService,
    private readonly _logger: Logger
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult<{ userId: string; userType?: 'student' | 'teacher' }, string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user) || user.isStudent()) return Result.failed(this._strings.errors.noCurrentUser);
    return Result.failed(this._strings.errors.noCurrentUser);

    // const studentType = user.isStudent() ? 'student' : undefined;
    // const teacherType = user.isTeacher() ? 'teacher' : undefined;
    // return Result.success({ userId: user.userId, userType: studentType ?? teacherType });
  }

  async perform(): Promise<SuccessOrFailedResult> {
    const canPerform = await this.canPerform();
    if (canPerform.isFailed()) {
      this._snackBarService.showMessage(canPerform.reason);
      return Result.failed();
    }
    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new SaveUserLogCommand(this._logger.getLog().join("\n\r"))));
    }
    catch(e){
      console.log(e);
    }
    finally {
      this._loaderService.hide();
    }

    await this._dialog.show(InformationDialog, new InformationDialogData(this._strings.sendLogs, this._strings.success));
    return Result.success();
  }


}
