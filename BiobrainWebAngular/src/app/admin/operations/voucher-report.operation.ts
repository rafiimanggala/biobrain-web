import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { AppEventProvider } from '../../core/app/app-event-provider.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { UsageReportDialogComponent } from '../dialogs/usage-report-dialog/usage-report-dialog.component';
import { GetUsageReport } from 'src/app/api/reports/get-usage-report.query';
import { GetVouchersReport } from 'src/app/api/reports/get-vouchers-report.query';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import moment from 'moment';

@Injectable({
  providedIn: 'root',
})
export class VoucherReportOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _appEvent: AppEventProvider
  ) {
  }

  public async perform(): Promise<SuccessOrFailedResult> {

    const dialogResult = await this._dialog.show(UsageReportDialogComponent, undefined);
    if (!dialogResult.hasData()) return Result.failed();

    try {
      const timeZoneId = Intl.DateTimeFormat().resolvedOptions().timeZone;
      var result = await firstValueFrom(this._api.send(new GetVouchersReport(dialogResult.data.from.toJSON(), dialogResult.data.to.toJSON(), timeZoneId)));
      if (!result.fileUrl) return Result.failed();;
      const anchor = document.createElement('a');
      anchor.download = `Vouchers-Report_${moment().format('YYYY_MM_DD')}.csv`;
      anchor.href = result.fileUrl;
      anchor.click();
      return Result.success();
    } catch (e) {
      this._appEvent.errorEmit(this._strings.unableToGetUsageReport);
      return Result.failed();
    }
  }
}
