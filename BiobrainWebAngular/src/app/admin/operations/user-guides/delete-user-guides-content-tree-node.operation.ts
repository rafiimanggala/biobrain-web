import { Injectable } from '@angular/core';

import { StringsService } from 'src/app/share/strings.service';
import { Api } from 'src/app/api/api.service';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { UserGuideNodeViewModel } from '../../user-guides/view-models/user-guide-node.view-model';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { DeleteUserGuideNodeCommand } from 'src/app/api/user-guides/delete-user-guide-node.command';

@Injectable({
  providedIn: 'root',
})
export class DeleteUserGuidesContentTreeNodeOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider,
    private readonly _dialog: Dialog,
  ) {
  }

  public async perform(node: UserGuideNodeViewModel): Promise<SuccessOrFailedResult> {

    var result = await this._dialog.show(DeleteConfirmationDialog, new DeleteConfirmationDialogData(this._strings.userGuides, node.name))
    if (!result.data?.confirmed) return Result.failed();

    try {
      var req = await firstValueFrom(this._api.send(new DeleteUserGuideNodeCommand(node.nodeId)));
      if (!req) return Result.failed();;
     
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.errors.errorSavingDataOnServer);
      return Result.failed();
    }
  }
}
