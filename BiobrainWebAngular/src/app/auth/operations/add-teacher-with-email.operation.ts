import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { AddTeacherByEmailDialogComponent } from 'src/app/admin/dialogs/add-teacher-by-email-dialog/add-teacher-by-email-dialog.component';
import { AddTeacherByEmailCommand, AddTeacherByEmailCommand_Result } from 'src/app/api/teachers/add-teacher.command';
import { BadRequestCommonException } from 'src/app/core/exceptions/bad-request-common.exception';
import { InternalServerException } from 'src/app/core/exceptions/internal-server.exception';
import { RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';

import { ChangePasswordCommand } from '../../api/accounts/change-password.command';
import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { CurrentUserService } from '../services/current-user.service';

@Injectable({
  providedIn: 'root',
})
export class AddTeacherWithEmailOperation {
  addTeacher$: ReplaySubject<AddTeacherByEmailCommand_Result>;

  constructor(
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _snackBarService: SnackBarService,
    private readonly _appEvent: AppEventProvider
  ) {
    this.addTeacher$ = new ReplaySubject<AddTeacherByEmailCommand_Result>(1);
  }

  public async canPerform(): Promise<FailedOrSuccessResult<string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) return Result.failed(this._strings.errors.noCurrentUser);
    if (!user.isSysAdmin() && !user.isSchoolAdmin()) return Result.failed(this._strings.errors.userIsNotSystemAdministrator);

    return Result.success();
  }

  public async perform(schoolId: string): Promise<SuccessOrFailedResult> {
    const canPerformResult = await this.canPerform();
    if (canPerformResult.isFailed()) {
      this._snackBarService.showMessage(canPerformResult.reason);
      return Result.failed();
    }

    const dialogResult = await this._dialog.show(AddTeacherByEmailDialogComponent, undefined);
    if (!dialogResult.hasData()) return Result.failed();

    try {
      var result = await firstValueFrom(this._api.send(new AddTeacherByEmailCommand(schoolId, dialogResult.data.email)));
      this._snackBarService.showMessage(this._strings.addTeacherSuccessMessage(dialogResult.data.email));
      this.addTeacher$.next(result);
      return Result.success();
    } catch (e) {
      if (e instanceof BadRequestCommonException) this._appEvent.errorEmit(e.message);
      else if (e instanceof InternalServerException) this._appEvent.errorEmit(e.message);
      else if (e instanceof RequestValidationException) this._appEvent.errorEmit(validationExceptionToString(e));
      else this._appEvent.errorEmit(this._strings.error);
      return Result.failed();
    }
  }
}
