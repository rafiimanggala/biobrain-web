import { Injectable } from '@angular/core';

import { ResetPasswordCommand } from '../../api/accounts/reset-password.command';
import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ConfirmationDialog } from '../../share/dialogs/confirmation/confirmation.dialog';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';

@Injectable({
  providedIn: 'root',
})
export class ResetPasswordOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
  ) {
  }

  public async perform(userId: string): Promise<SuccessOrFailedResult> {
    const dialogResult = await this._dialog.show(ConfirmationDialog, {
      title: this.strings.resetPassword,
      text: this.strings.messages.areYouSureWantToResetPassword,
    });

    if (dialogResult.action !== DialogAction.yes) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new ResetPasswordCommand(userId)));
      this._snackBarService.showMessage(this.strings.resetEmailSuccessMessage);
      return Result.success();
    } catch (e) {
      const msg = 'error' in e ? (e as { error: string }).error : e as string;
      this._appEvents.errorEmit(msg);
      return Result.failed();
    }
  }
}
