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
import { UserGuideNodeViewModel } from '../../user-guides/view-models/user-guide-node.view-model';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { DeleteUserGuidesContentTreeNodeOperation } from './delete-user-guides-content-tree-node.operation';

@Injectable({
  providedIn: 'root',
})
export class EditUserGuidesContentTreeNodeOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider,
    private readonly _deleteUserGuidesContentTreeNodeOperation: DeleteUserGuidesContentTreeNodeOperation,
    private readonly _dialog: Dialog,
  ) {
  }

  public async perform(node: UserGuideNodeViewModel): Promise<SuccessOrFailedResult> {

    const dialogResult = await this._dialog.show(UserGuideNodeDialog, new UserGuideNodeDialogData(node.name, node.isAvailableForStudent, node.parentId == undefined || node.parentId.length < 1, true));
    if (!dialogResult.hasData()) return Result.failed();

    if (dialogResult.action == DialogAction.save) {
      try {
        var result = await firstValueFrom(this._api.send(new SaveUserGuideNodeCommand(node.nodeId, node.parentId, dialogResult.data.name, dialogResult.data.isAvailableForStudent)));
        if (!result) return Result.failed();
      } catch (e) {
        this._appEvent.errorEmit(this._strings.errors.errorSavingDataOnServer);
        return Result.failed();
      }
    }
    if(dialogResult.action == DialogAction.delete){
      await this._deleteUserGuidesContentTreeNodeOperation.perform(node);
    }
    return Result.success();
  }
}
