import { Injectable } from '@angular/core';
import { Moment } from 'moment';
import { UpdateDueDateForQuizAssignmentCommand } from 'src/app/api/quiz-assignments/update-due-date-for-quiz-assignment.command';
import { PickDateDialogData } from 'src/app/share/dialogs/pick-date-dialog/pick-date-dialog-data';
import { PickDateDialogComponent } from 'src/app/share/dialogs/pick-date-dialog/pick-date-dialog.component';

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
export class UpdateDueDateForQuizOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvents: AppEventProvider,
  ) {
  }

  public async perform(assignmentId: string, dueDate: Moment): Promise<SuccessOrFailedResult<void>> {

    const dialogResult = await this._dialog.show(PickDateDialogComponent, new PickDateDialogData(this.strings.dueDate, dueDate));

    if (dialogResult.action !== DialogAction.save || !dialogResult.data?.date) return Result.failed();

    try {
      await firstValueFrom(this._api.send(new UpdateDueDateForQuizAssignmentCommand(assignmentId, dialogResult.data.date, dialogResult.data.date.utc())));
      return Result.success();
    } catch (e: any) {
      const msg = 'error' in e ? (e as { error: string }).error : e as string;
      this._appEvents.errorEmit(msg);
      return Result.failed();
    }
  }
}
