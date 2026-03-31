import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { DeleteAccountPemanentCommand } from 'src/app/api/accounts/delete-account-permanent.command';

@Injectable({
  providedIn: 'root',
})
export class PermanentDeleteUserOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvent: AppEventProvider
  ) {
  }

  public async perform(userId: string, email: string): Promise<SuccessOrFailedResult> {

    const dialogResult = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this._strings.permanentDeleteUser, this._strings.permanentDeleteUserConfirmation(email)));
    if (dialogResult.action != DialogAction.yes) return Result.success();

    try {
      await firstValueFrom(this._api.send(new DeleteAccountPemanentCommand(userId)));
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.unableToChangePassword);
      return Result.failed();
    }
  }
}
