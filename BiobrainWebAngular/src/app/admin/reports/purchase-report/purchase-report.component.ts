import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDatepicker } from '@angular/material/datepicker';
import moment from 'moment';
import { Api } from 'src/app/api/api.service';
import { GetPurchaseReportToCsv } from 'src/app/api/reports/get-purchase-report-to-csv.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { StringsService } from 'src/app/share/strings.service';

@Component({
  selector: 'app-purchase-report',
  templateUrl: './purchase-report.component.html',
  styleUrls: ['./purchase-report.component.scss'],
})
export class PurchaseReportComponent extends BaseComponent {

  @ViewChild('fromPicker') fromDatePicker: MatDatepicker<moment.Moment> | undefined;
  @ViewChild('toPicker') toDatePicker: MatDatepicker<moment.Moment> | undefined;
  fromDate = new FormControl(moment().startOf('day').subtract(1, 'month'));
  toDate = new FormControl(moment().endOf('day'));
  
  constructor(
    private readonly _api: Api,
    public readonly strings: StringsService,
    appEvents: AppEventProvider
  ) { super(appEvents); }

  async onGetReport() {
      try {
          const timeZoneId = Intl.DateTimeFormat().resolvedOptions().timeZone;
          const from = (<moment.Moment>this.fromDate.value).startOf('day').toJSON();
          const to = (<moment.Moment>this.toDate.value).endOf('day').toJSON();
          const result = await firstValueFrom(this._api.send(new GetPurchaseReportToCsv(from, to, timeZoneId)));

          if(!result.fileUrl) return;
          const anchor = document.createElement('a');
          anchor.download = `purchase-report_${moment().format('YYYY_MM_DD')}.csv`;
          anchor.href = result.fileUrl;
          anchor.click();
      }
      catch (e) {
          throw e;
      }
  }

  fromDatepickerClick() {
    this.fromDatePicker?.open();
  }

  toDatepickerClick() {
    this.toDatePicker?.open();
  }
}

