import { Injectable } from '@angular/core';

import { ChangeSelfPasswordCommand } from '../../api/accounts/change-self-password.command';
import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { ChangeSelfPasswordDialogComponent } from '../dialogs/change-self-password-dialog/change-self-password-dialog.component';
import { CurrentUserService } from '../services/current-user.service';

@Injectable({
  providedIn: 'root',
})
export class ChangeSelfPasswordOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _snackBarService: SnackBarService,
    private readonly _appEvent: AppEventProvider
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult<{userId: string}, string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) return Result.failed(this._strings.errors.noCurrentUser);

    return Result.success({ userId: user.userId });
  }

  public async perform(): Promise<SuccessOrFailedResult> {
    const canPerformResult = await this.canPerform();
    if (canPerformResult.isFailed()) {
      this._snackBarService.showMessage(canPerformResult.reason);
      return Result.failed();
    }

    const { userId } = canPerformResult.data;

    const dialogResult = await this._dialog.show(ChangeSelfPasswordDialogComponent, undefined);
    if (!dialogResult.hasData()) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new ChangeSelfPasswordCommand(userId, dialogResult.data.oldPassword, dialogResult.data.newPassword)));
      this._snackBarService.showMessage(this._strings.passwordHasBeenChanged);
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.unableToChangePassword);
      return Result.failed();
    }
  }
}
