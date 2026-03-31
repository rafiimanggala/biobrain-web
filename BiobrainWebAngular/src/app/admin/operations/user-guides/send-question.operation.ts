import { Injectable } from '@angular/core';

import { StringsService } from 'src/app/share/strings.service';
import { Api } from 'src/app/api/api.service';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { UserGuideNodeDialog } from '../../user-guides/dialogs/user-guide-node/user-guide-node-dialog.component';
import { UserGuideNodeDialogData } from '../../user-guides/dialogs/user-guide-node/user-guide-node-dialog-data';
import { SaveUserGuideNodeCommand } from 'src/app/api/user-guides/save-user-guide-node.command';
import { SendQuestionCommand } from 'src/app/api/user-guides/send-question.command';
import { HintDialogComponent } from 'src/app/learning-content/dialogs/hint-dialog/hint-dialog.component';
import { HintDialogData } from 'src/app/learning-content/dialogs/hint-dialog/hint-dialog-data';
import { AppHintDialog } from 'src/app/share/dialogs/app-hint/app-hint.dialog';
import { AppHintDialogData } from 'src/app/share/dialogs/app-hint/app-hint.dialog-data';
import { InformationDialog } from 'src/app/share/dialogs/information/information.dialog';
import { InformationDialogData } from 'src/app/share/dialogs/information/information.dialog-data';

@Injectable({
  providedIn: 'root',
})
export class SendQuestionOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider,
    private readonly _dialog: Dialog,
  ) {
  }

  public async perform(question: string): Promise<SuccessOrFailedResult> {

    try {
      var result = await firstValueFrom(this._api.send(new SendQuestionCommand(question)));
      if (!result) return Result.failed();

      await this._dialog.show(InformationDialog, new InformationDialogData(this._strings.biobrainFeedback, this._strings.questionFeedback));
     
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.errors.errorSavingDataOnServer);
      return Result.failed();
    }
  }
}
