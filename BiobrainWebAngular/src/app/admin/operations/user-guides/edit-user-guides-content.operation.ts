import { Injectable } from '@angular/core';

import { StringsService } from 'src/app/share/strings.service';
import { Api } from 'src/app/api/api.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { UserGuideContentViewModel } from '../../user-guides/view-models/user-guide-content.view-model';
import { SaveUserGuideContentCommand } from 'src/app/api/user-guides/save-user-guide-content.command';
import { LoaderService } from 'src/app/share/services/loader.service';

@Injectable({
  providedIn: 'root',
})
export class EditUserGuidesContentOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider,
    private readonly _loader: LoaderService
  ) {
  }

  public async perform(article: UserGuideContentViewModel): Promise<SuccessOrFailedResult> {

    try {
      this._loader.show();
      await firstValueFrom(this._api.send(new SaveUserGuideContentCommand(article.articleId, article.nodeId, article.htmlText, article.videoUrl)));
    } catch (e) {
      this._appEvent.errorEmit(this._strings.errors.errorSavingDataOnServer);
      return Result.failed();
    }
    finally{
      this._loader.hide();
    }
    return Result.success();
  }
}
