import { Injectable } from '@angular/core';

import { StringsService } from 'src/app/share/strings.service';
import { Api } from 'src/app/api/api.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { UserGuideNodeViewModel } from '../../user-guides/view-models/user-guide-node.view-model';
import { GetUserGuideContentTreeQuery, GetUserGuideContentTreeQuery_Result } from 'src/app/api/user-guides/get-user-guide-content-tree.query';

@Injectable({
  providedIn: 'root',
})
export class GetUserGuidesContentTreeOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider
  ) {
  }

  public async perform(): Promise<SuccessOrFailedResult<UserGuideNodeViewModel[]>> {

    try {
      var result = await firstValueFrom(this._api.send(new GetUserGuideContentTreeQuery()));
      if (!result) return Result.failed();;
     
      return Result.success(result.map(_ => this.mapViewModel(_)));
    } catch (e) {
      this._appEvent.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
      return Result.failed();
    }
  }

  private mapViewModel(model: GetUserGuideContentTreeQuery_Result): UserGuideNodeViewModel{
    return new UserGuideNodeViewModel(model.nodeId, model.parentId, model.name, model.order, model.isAvailableForStudent, model.children?.map(_ => this.mapViewModel(_)) ?? []);
  }
}
