import { Injectable } from '@angular/core';

import { ChangeEmailCommand } from '../../api/accounts/change-email.command';
import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { ChangeEmailDialogComponent } from '../dialogs/change-email-dialog/change-email-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class ChangeEmailOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _snackBarService: SnackBarService,
    private readonly _appEvent: AppEventProvider,
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult> {
    return Promise.resolve(Result.success());
  }

  public async perform(userId: string): Promise<SuccessOrFailedResult> {
    const dialogResult = await this._dialog.show(ChangeEmailDialogComponent, undefined);
    if (!dialogResult.hasData()) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new ChangeEmailCommand(userId, dialogResult.data.email)));
      this._snackBarService.showMessage(this._strings.emailHasBeenChanged);
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.unableToChangeEmail);
      return Result.failed();
    }
  }
}
