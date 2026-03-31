import { Component, OnInit } from '@angular/core';

import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { TitleCasePipe } from '@angular/common';
import { LoaderService } from 'src/app/share/services/loader.service';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { Api } from 'src/app/api/api.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import moment from 'moment';
import { VoucherViewModel } from './voucher.view-model';
import { VoucherDialog } from '../../dialogs/voucher-dialog/voucher-dialog.component';
import { VoucherDialogData } from '../../dialogs/voucher-dialog/voucher-dialog-data';
import { GenerateVoucherCommand } from 'src/app/api/vouchers/generate-voucher.command';
import { GetVouchersQuery } from 'src/app/api/vouchers/get-vouchers.query';
import { alphaNumericStringComparator } from 'src/app/share/helpers/alpha-numeric-comparer';
import { GridApi, GridOptions } from 'ag-grid-community';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { DeleteVoucherCommand } from 'src/app/api/vouchers/delete-voucher.command';
import { VoucherReportOperation } from '../../operations/voucher-report.operation';

@Component({
  selector: 'app-voucher-list',
  templateUrl: './voucher-list.component.html',
  styleUrls: ['./voucher-list.component.scss'],
})
export class VoucherListComponent extends DisposableSubscriberComponent implements OnInit {
  readonly strComparator = alphaNumericStringComparator;
  private _gridApi: GridApi | null | undefined;
  nameFilterParams = {
    filterOptions: ['contains', 'notContains'],
    trimInput: true,
    debounceMs: 200,
  };
  vouchers: VoucherViewModel[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _loaderService: LoaderService,
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _appEventsService: AppEventProvider,
    private readonly _voucherReportOperation: VoucherReportOperation
  ) {
    super();
  }

  async ngOnInit() {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(`${this._titlecasePipe.transform(this.strings.vouchers)}`);
    }, 0);

    await this.getCodesInternal();
  }

  onGridReady(params: GridOptions): void {
    this._gridApi = params.api;
    this._gridApi?.sizeColumnsToFit();
  }

  onModelUpdated(): void {
    this._gridApi?.sizeColumnsToFit();
  }

  async onCreateVoucher() {

    var result = await this._dialog.show(VoucherDialog, new VoucherDialogData());
    if (result.action != DialogAction.save || !result.data) return;


    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new GenerateVoucherCommand(result.data.note, result.data.totalAmount, result.data.country, result.data.expiryDate.utc().toJSON(), result.data.redeemExpiryDate.utc().toJSON(), result.data.numberOfCodes)));
    }
    catch (e: any) {
      this._appEventsService.errorEmit(e.message);
      return;
    }
    finally {
      this._loaderService.hideIfVisible();
    }

    await this.getCodesInternal();
  }

  async onDelete(voucher: VoucherViewModel) {
    if (!voucher || !voucher.voucherId) return;

    var result = await this._dialog.show(DeleteConfirmationDialog, new DeleteConfirmationDialogData(this.strings.voucher, voucher.code))
    if (!result.data?.confirmed) return;

    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new DeleteVoucherCommand(voucher.voucherId)));
    }
    catch (e: any) {
      this._appEventsService.errorEmit(e.message);
      return;
    }
    finally {
      this._loaderService.hideIfVisible();
    }

    await this.getCodesInternal();
  }

  async downloadAccessCodes() {
    this.dyanmicDownloadByHtmlTag(`${this.strings.vouchers} ${moment().format("DD-MMM-YYYY")}`, this.vouchers.map(_ => `${_.code},${_.note.replace(',','')},${_.totalAmount}`).join("\n"));
  }

  formatDate(dateUtc: string | null): string {
    if (!dateUtc) return '';
    return moment.utc(dateUtc).local().format("DD-MMM-YYYY");
  }

  formatMoney(node: any): string {
    let amount = node.value;
    if (!amount) return '0$';
    return `${amount.toString()}$`;
  }

  private async getCodesInternal() {
    try {
      this._loaderService.show();
      var result = await firstValueFrom(this._api.send(new GetVouchersQuery()));
      this.vouchers = result.map(_ => {
        return {
          voucherId: _.voucherId,
          code: _.code,
          note: _.note,
          totalAmount: _.totalAmount,
          amountUsed: _.amountUsed,
          country: _.country,
          expiryDateUtc: _.expiryDateUtc,
          expiryDateLocal: moment.utc(_.expiryDateUtc).local(),
          redeemExpiryDateUtc: _.redeemExpiryDateUtc,
          redeemExpiryDateLocal: moment.utc(_.redeemExpiryDateUtc).local(),
          createdAtUtc: _.createdAt
        }
      });
    }
    catch (e: any) {
      this._appEventsService.errorEmit(e.message);
      return;
    }
    finally {
      this._loaderService.hideIfVisible();
    }
  }

  private dyanmicDownloadByHtmlTag(fileName: string, text: string) {
    const element = document.createElement('a');
    const fileType = 'text/csv';
    element.setAttribute('href', `data:${fileType};charset=utf-8,${encodeURIComponent(text)}`);
    element.setAttribute('download', fileName);

    var event = new MouseEvent("click");
    element.dispatchEvent(event);
    element.remove();
  }

  async onGetVouchersReport(){
    await this._voucherReportOperation.perform();
  }
}
