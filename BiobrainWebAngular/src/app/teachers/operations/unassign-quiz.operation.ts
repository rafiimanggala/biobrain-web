import { Injectable } from '@angular/core';
import { UnassignQuizToClassCommand } from 'src/app/api/quiz-assignments/unassign-quiz-to-class.command';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';

@Injectable({
  providedIn: 'root',
})
export class UnassignQuizOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
  ) {
  }

  public async perform(assignmentId: string): Promise<SuccessOrFailedResult<void>> {

    const dialogResult = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this.strings.unassign, this.strings.messages.unassignConfirmation));

    if (dialogResult.action !== DialogAction.yes) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new UnassignQuizToClassCommand(assignmentId)));
      return Result.success();
    } catch (e: any) {
      const msg = 'error' in e ? (e as { error: string }).error : e as string;
      this._appEvents.errorEmit(msg);
      return Result.failed();
    }
  }
}
