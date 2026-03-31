import { Injectable } from '@angular/core';

import { ResetSelfPasswordCommand } from '../../api/accounts/reset-self-password.command';
import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';

@Injectable({
  providedIn: 'root',
})
export class ResetSelfPasswordOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
  ) {
  }

  public async perform(email: string): Promise<SuccessOrFailedResult> {
    try {
      await firstValueFrom(this._api.send(new ResetSelfPasswordCommand(email)));
      this._snackBarService.showMessage(this.strings.selfResetEmailSuccessMessage);
      return Result.success();
    } catch (e) {
      const _ = 'error' in e ? (e as { error: string }).error : e as string;
      this._appEvents.errorEmit(this.strings.unableToResetPassword);
      return Result.failed();
    }
  }
}
