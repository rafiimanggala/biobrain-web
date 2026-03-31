import { Injectable } from '@angular/core';
import { EmailSchoolClassCommand } from 'src/app/api/students/email-school-class.command';
import { RenameClassCommand } from 'src/app/api/students/rename-class.command';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { EmailClassDialog } from '../dialogs/email-class/email-class.dialog';
import { RenameClassDialog } from '../dialogs/rename-class/rename-class.dialog';

@Injectable({
  providedIn: 'root',
})
export class EmailClassOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
    private readonly _snackBarService: SnackBarService,
  ) {
  }

  public async perform(classId: string): Promise<SuccessOrFailedResult> {

    const dialogResult = await this._dialog.show(EmailClassDialog, {
      text: '',
    });

    if (dialogResult.action !== DialogAction.yes || !dialogResult.data?.text) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new EmailSchoolClassCommand(dialogResult.data.text, classId)));
      this._snackBarService.showMessage(this.strings.messages.emailWasSent);
      return Result.success();
    } catch (e) {
      const msg = 'error' in e ? (e as { error: string }).error : e as string;
      this._appEvents.errorEmit(msg);
      return Result.failed();
    }
  }
}
