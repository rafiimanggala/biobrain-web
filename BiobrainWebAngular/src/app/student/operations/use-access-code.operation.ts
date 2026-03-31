import { Injectable } from '@angular/core';
import { UseAccessCodeCommand } from 'src/app/api/accounts/use-access-code.command';
import { RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';
import { UnprocessableEntityException } from 'src/app/core/exceptions/unprocessable-entity.exception';

import { Api } from '../../api/api.service';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { UseAccessCodeDialogComponent } from '../dialogs/use-access-code-dialog/use-access-code-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class UseAccessCodeOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _snackBarService: SnackBarService,
  ) { }

  public async canPerform(): Promise<SuccessOrFailedResult<{userId: string}, string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) return Result.failed(this._strings.errors.noCurrentUser);
    if (!user.isStudent()) return Result.failed(this._strings.errors.userIsNotStudent);
    // if (user.isStandalone()) return Result.failed(this._strings.errors.userIsNotStudent);

    return Result.success({ userId: user.userId });
  }

  public async perform(): Promise<void> {
    const canPerformResult = await this.canPerform();
    if (canPerformResult.isFailed()) {
      this._snackBarService.showMessage(canPerformResult.reason);
      return;
    }

    const { userId } = canPerformResult.data;

    const dialogResult = await this._dialog.show(UseAccessCodeDialogComponent, undefined);
    if (!dialogResult.hasData()) return;

    try {
      await firstValueFrom(this._api.send(new UseAccessCodeCommand(userId, dialogResult.data.accessCode)));
      this._snackBarService.showMessage(this._strings.success);
    } 
    catch (e) {
      if(e instanceof RequestValidationException){
        await this._dialog.show(ErrorMessageDialogComponent, { text: validationExceptionToString(e) });
        return;
      }
      if(e instanceof UnprocessableEntityException){
        if(e.name == 'NotEnoughStudentsLicensesException')
          this._snackBarService.showMessage(this._strings.studentsLicenseLimitExceeded);
        else
          this._snackBarService.showMessage(e.name);
        return;
      }
      await this._dialog.show(ErrorMessageDialogComponent, { text: this._strings.wrongClassCode });
    }
  }
}
