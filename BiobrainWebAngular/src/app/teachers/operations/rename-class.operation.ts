import { Injectable } from '@angular/core';
import { RenameClassCommand } from 'src/app/api/students/rename-class.command';
import { isValidationError, RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { RenameClassDialog } from '../dialogs/rename-class/rename-class.dialog';

@Injectable({
  providedIn: 'root',
})
export class RenameClassOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
  ) {
  }

  public async perform(className: string, classId: string): Promise<SuccessOrFailedResult<string>> {

    const dialogResult = await this._dialog.show(RenameClassDialog, {
      className: className,
    });

    if (dialogResult.action !== DialogAction.yes || !dialogResult.data?.className) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new RenameClassCommand(dialogResult.data.className, classId)));
      this._snackBarService.showMessage(this.strings.renameClassSuccessMessage(className));
      return Result.success(dialogResult.data.className);
    } catch (e: any) {
      if (e instanceof RequestValidationException) {
        this._appEvents.errorEmit(validationExceptionToString(e));
      } else {
        const msg = 'error' in e ? (e as { error: string }).error : e as string;
        this._appEvents.errorEmit(msg);
      }
      return Result.failed();
    }
  }
}
