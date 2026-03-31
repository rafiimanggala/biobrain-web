import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { UsageReportDialogComponent } from '../dialogs/usage-report-dialog/usage-report-dialog.component';
import { GetUsageReport } from 'src/app/api/reports/get-usage-report.query';
import moment from 'moment';

@Injectable({
  providedIn: 'root',
})
export class UsageReportOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvent: AppEventProvider
  ) {
  }

  public async perform(schoolId: string): Promise<SuccessOrFailedResult> {

    const dialogResult = await this._dialog.show(UsageReportDialogComponent, undefined);
    if (!dialogResult.hasData()) return Result.failed();

    try {
      const timeZoneId = Intl.DateTimeFormat().resolvedOptions().timeZone;
      var result = await firstValueFrom(this._api.send(new GetUsageReport(schoolId, dialogResult.data.from.toJSON(), dialogResult.data.to.toJSON(), timeZoneId)));
      if (!result.fileUrl) return Result.failed();;
      const anchor = document.createElement('a');
      anchor.download = `Usage-Report_${moment().format('YYYY_MM_DD')}.pdf`;
      anchor.href = result.fileUrl;
      anchor.click();
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.unableToGetUsageReport);
      return Result.failed();
    }
  }
}
