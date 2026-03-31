import { Injectable } from '@angular/core';

import { ChangeSelfEmailCommand } from '../../api/accounts/change-self-email.command';
import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { RequestValidationException } from '../../core/exceptions/request-validation.exception';
import { UnprocessableEntityException } from '../../core/exceptions/unprocessable-entity.exception';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { ChangeSelfEmailDialogComponent } from '../dialogs/change-self-email-dialog/change-self-email-dialog.component';
import { CurrentUserService } from '../services/current-user.service';

import { RefreshAuthorizationOperation } from './refresh-authorization.operation';

@Injectable({
  providedIn: 'root',
})
export class ChangeSelfEmailOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _snackBarService: SnackBarService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _appEvent: AppEventProvider,
    private readonly _refreshAuthorizationOperation: RefreshAuthorizationOperation,
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult<{ userId: string; email: string }, string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) return Result.failed(this._strings.errors.noCurrentUser);

    return Result.success({
      userId: user.userId,
      email: user.name,
    });
  }

  public async perform(): Promise<SuccessOrFailedResult> {
    const canPerform = await this.canPerform();
    if (canPerform.isFailed()) {
      this._snackBarService.showMessage(canPerform.reason);
      return Result.failed();
    }

    const {
      userId,
      email,
    } = canPerform.data;

    const dialogResult = await this._dialog.show(ChangeSelfEmailDialogComponent, { email });
    if (!dialogResult.hasData()) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new ChangeSelfEmailCommand(userId, dialogResult.data.email, dialogResult.data.password)));
      await this._refreshAuthorizationOperation.perform();
      this._snackBarService.showMessage(this._strings.emailHasBeenChanged);
      return Result.success();
    } catch (e) {
      if (e instanceof UnprocessableEntityException && e.name === 'IncorrectPasswordException') {
        this._appEvent.errorEmit(this._strings.incorrectPassword);
        return Result.failed();
      }
      if (e instanceof RequestValidationException) {
        for (const key in e.errors) this._appEvent.errorEmit(e.errors[key].join(', '));
        return Result.failed();
      }
      this._appEvent.errorEmit(this._strings.unableToChangeEmail);
      return Result.failed();
    }
  }
}
