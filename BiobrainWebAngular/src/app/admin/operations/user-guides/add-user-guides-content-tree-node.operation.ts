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

@Injectable({
  providedIn: 'root',
})
export class AddUserGuidesContentTreeNodeOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider,
    private readonly _dialog: Dialog,
  ) {
  }

  public async perform(parentId: string|undefined): Promise<SuccessOrFailedResult> {

    const dialogResult = await this._dialog.show(UserGuideNodeDialog, new UserGuideNodeDialogData("", false, parentId == undefined || parentId.length < 1 ));
    if (!dialogResult.hasData()) return Result.failed();

    try {
      var result = await firstValueFrom(this._api.send(new SaveUserGuideNodeCommand(null, parentId ?? null, dialogResult.data.name, dialogResult.data.isAvailableForStudent)));
      if (!result) return Result.failed();;
     
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.errors.errorSavingDataOnServer);
      return Result.failed();
    }
  }
}
