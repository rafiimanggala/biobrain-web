import { Injectable } from '@angular/core';

import { ChangePasswordCommand } from '../../api/accounts/change-password.command';
import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { ChangePasswordDialogComponent } from '../dialogs/change-password-dialog/change-password-dialog.component';
import { CurrentUserService } from '../services/current-user.service';

@Injectable({
  providedIn: 'root',
})
export class ChangePasswordOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _snackBarService: SnackBarService,
    private readonly _appEvent: AppEventProvider
  ) {
  }

  public async canPerform(): Promise<FailedOrSuccessResult<string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) return Result.failed(this._strings.errors.noCurrentUser);
    if (!user.isSysAdmin()) return Result.failed(this._strings.errors.userIsNotSystemAdministrator);

    return Result.success();
  }

  public async perform(userId: string): Promise<SuccessOrFailedResult> {
    const canPerformResult = await this.canPerform();
    if (canPerformResult.isFailed()) {
      this._snackBarService.showMessage(canPerformResult.reason);
      return Result.failed();
    }

    const dialogResult = await this._dialog.show(ChangePasswordDialogComponent, undefined);
    if (!dialogResult.hasData()) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new ChangePasswordCommand(userId, dialogResult.data.newPassword)));
      this._snackBarService.showMessage(this._strings.passwordHasBeenChanged);
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.unableToChangePassword);
      return Result.failed();
    }
  }
}
