import { Injectable } from '@angular/core';
import { SetPasswordCommand } from '../../api/accounts/set-password.command';
import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';

@Injectable({
  providedIn: 'root'
})
export class SetPasswordOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
  ) { }

  public async perform(login: string, token: string, password: string): Promise<SuccessOrFailedResult> {
    try {
      await firstValueFrom(this._api.send(new SetPasswordCommand(login, token, password)));
      this._snackBarService.showMessage(this.strings.passwordHasBeenChanged);
      return Result.success();
    } catch (e) {
      const msg = 'errors' in e ? (e as { errors: string[] }).errors[0] : e as string;
      this._appEvents.errorEmit(msg);
      return Result.failed();
    }
  }
}
