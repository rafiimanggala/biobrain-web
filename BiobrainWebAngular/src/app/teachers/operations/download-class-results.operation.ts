import { Injectable } from '@angular/core';
import { GetClassResultQuery } from 'src/app/api/teachers/get-class-results-csv.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';

import { Api } from '../../api/api.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';

@Injectable({
  providedIn: 'root',
})
export class DownloadClassResultsOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvents: AppEventProvider
  ) {
  }

  public async perform(courseId: string, schoolClassId: string): Promise<SuccessOrFailedResult<string>> {
    try {
      var result =  await firstValueFrom(this._api.send(new GetClassResultQuery(courseId, schoolClassId)));
      return Result.success(result.fileUrl);
    } catch (e) {
      const msg = 'error' in e ? (e as { error: string }).error : e as string;
      this._appEvents.errorEmit(msg);
      return Result.failed();
    }
  }
}
