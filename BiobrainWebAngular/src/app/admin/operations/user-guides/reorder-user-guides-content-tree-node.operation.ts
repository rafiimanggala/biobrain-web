import { Injectable } from '@angular/core';

import { StringsService } from 'src/app/share/strings.service';
import { Api } from 'src/app/api/api.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { UserGuideNodeViewModel } from '../../user-guides/view-models/user-guide-node.view-model';
import { ReorderUserGuideNodeCommand } from 'src/app/api/user-guides/reorder-user-guide-node.command';

@Injectable({
  providedIn: 'root',
})
export class ReorderUserGuidesContentTreeNodeOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider,
  ) {
  }

  public async perform(node: UserGuideNodeViewModel): Promise<SuccessOrFailedResult> {

    try {
      var req = await firstValueFrom(this._api.send(new ReorderUserGuideNodeCommand(node.nodeId, node.order)));
      if (!req) return Result.failed();;
     
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.errors.errorSavingDataOnServer);
      return Result.failed();
    }
  }
}
