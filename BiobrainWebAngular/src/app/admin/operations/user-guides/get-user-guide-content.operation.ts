import { Injectable } from '@angular/core';

import { StringsService } from 'src/app/share/strings.service';
import { Api } from 'src/app/api/api.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { UserGuideContentViewModel } from '../../user-guides/view-models/user-guide-content.view-model';
import { GetUserGuideContentQuery } from 'src/app/api/user-guides/get-user-guide-content.query';
import { LoaderService } from 'src/app/share/services/loader.service';

@Injectable({
  providedIn: 'root',
})
export class GetUserGuideContentOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider,
    private readonly _loader: LoaderService
  ) {
  }

  public async perform(nodeId: string): Promise<SuccessOrFailedResult<UserGuideContentViewModel>> {

    try {
      this._loader.show();
      var result = await firstValueFrom(this._api.send(new GetUserGuideContentQuery(nodeId)));
      if (!result) return Result.failed();;
     
      return Result.success(new UserGuideContentViewModel(result.articleId, result.nodeId, result.htmlText ?? '', result.videoUrl ?? ''));
    } catch (e) {
      this._appEvent.errorEmit(this._strings.errors.errorRetrivingDataFromServer);
      return Result.failed();
    }
    finally{
      this._loader.hide();
    }
  }
}
