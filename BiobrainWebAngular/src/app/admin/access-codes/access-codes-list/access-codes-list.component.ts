import { Component, OnInit } from '@angular/core';

import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { TitleCasePipe } from '@angular/common';
import { LoaderService } from 'src/app/share/services/loader.service';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { EditBatchDialog } from '../../dialogs/edit-batch-dialog/edit-batch-dialog.component';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { Api } from 'src/app/api/api.service';
import { GetCoursesListQuery } from 'src/app/api/content/get-courses-list.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { EditBatchDialogData } from '../../dialogs/edit-batch-dialog/edit-batch-dialog-data';
import { EditBatchDialogDataCourse } from '../../dialogs/edit-batch-dialog/edit-batch-dialog-data-course';
import { GenerateAccessCodesCommand } from 'src/app/api/access-codes/generate-access-codes.command';
import { GetAccessCodesQuery, GetAccessCodesQuery_Result_Batch } from 'src/app/api/access-codes/get-access-codes.query';
import { PageEvent } from '@angular/material/paginator';
import moment, { Moment } from 'moment';
import { DeleteAccessCodesCommand } from 'src/app/api/access-codes/delete-access-codes.command';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { AccessCodeBatchViewModel } from './access-code-batch.view-model';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';
import { UpdateAccessCodeBatchExpiryDateCommand } from 'src/app/api/access-codes/update-access-code-batch-expiry-date.command';
import { GetAccessCodesBatchReportQuery } from 'src/app/api/access-codes/get-access-codes-batch-report.query';

@Component({
  selector: 'app-access-codes-list',
  templateUrl: './access-codes-list.component.html',
  styleUrls: ['./access-codes-list.component.scss'],
})
export class AccessCodesListComponent extends DisposableSubscriberComponent implements OnInit {
  pageNumber: number = 1;
  pageSize: number = 20;
  totalLength: number = 0;
  batches: AccessCodeBatchViewModel[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _loaderService: LoaderService,
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _appEventsService: AppEventProvider
  ) {
    super();
  }

  async ngOnInit() {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(`${this._titlecasePipe.transform(this.strings.accessCodes)}`);
    }, 0);

    await this.getCodesInternal();
  }

  async onCreateBatch() {
    try {
      this._loaderService.show();
      var courses = await firstValueFrom(this._api.send(new GetCoursesListQuery()));
    }
    catch (e: any) {
      this._appEventsService.errorEmit(e.message);
      return;
    }
    finally {
      this._loaderService.hideIfVisible();
    }

    var result = await this._dialog.show(EditBatchDialog, new EditBatchDialogData(courses.map(_ => new EditBatchDialogDataCourse(_.courseId, _.name, false))));
    if (result.action != DialogAction.save || !result.data) return;


    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new GenerateAccessCodesCommand(result.data.note, result.data.courseIds, result.data.numberOfCodes, result.data.expiryDate?.utc().toJSON() ?? '')));
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

  async onDeleteBatch(batch: GetAccessCodesQuery_Result_Batch) {
    if (!batch || !batch.batchId) return;

    var result = await this._dialog.show(DeleteConfirmationDialog, new DeleteConfirmationDialogData(this.strings.batch, batch.batchHeader))
    if (!result.data?.confirmed) return;

    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new DeleteAccessCodesCommand(batch.batchId)));
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

  async onPageChanged(event: PageEvent) {
    this.pageNumber = event.pageIndex;
    await this.getCodesInternal();
  }

  async downloadAccessCodes(batch: GetAccessCodesQuery_Result_Batch) {
    this.dyanmicDownloadByHtmlTag(`${batch.batchHeader} ${moment().format("DD-MMM-YYYY")}`, batch.codes.map(_ => _.code).join("\n"));
  }

  formatDate(dateUtc: string | null): string {
    if (!dateUtc) return '';
    return moment.utc(dateUtc).local().format("DD-MMM-YYYY");
  }

  private async getCodesInternal() {
    try {
      this._loaderService.show();
      var result = await firstValueFrom(this._api.send(new GetAccessCodesQuery(this.pageNumber, this.pageSize)));
      this.batches = result.batches.map(_ => {
        return {
          batchId: _.batchId,
          batchHeader: _.batchHeader,
          codes: _.codes,
          usedCodes: _.usedCodes,
          expiryDateUtc: _.expiryDateUtc,
          expiryDateLocal: moment.utc(_.expiryDateUtc).local(),
          createdAtUtc: _.createdAtUtc
        }
      });
      this.totalLength = result.totalLength;
    }
    catch (e: any) {
      this._appEventsService.errorEmit(e.message);
      return;
    }
    finally {
      this._loaderService.hideIfVisible();
    }
  }

  async getAccessCodesBatchReport(batch: GetAccessCodesQuery_Result_Batch){
    try {
      this._loaderService.show();
      var result = await firstValueFrom(this._api.send(new GetAccessCodesBatchReportQuery(batch.batchId, Intl.DateTimeFormat().resolvedOptions().timeZone)));
      const anchor = document.createElement('a');
      anchor.download = `Used_Access_Codes_${batch.batchHeader}_${moment().format('YYYY_MM_DD')}.csv`;
      anchor.href = result.fileUrl;
      anchor.click();
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
    const fileType = 'text/plain';
    element.setAttribute('href', `data:${fileType};charset=utf-8,${encodeURIComponent(text)}`);
    element.setAttribute('download', fileName);

    var event = new MouseEvent("click");
    element.dispatchEvent(event);
    element.remove();
  }

  async onExpiryEditSelected(newValue: Moment, batch: AccessCodeBatchViewModel) {
    if (!batch || !batch.batchId) return;

    var result = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this.strings.batch, this.strings.expiryDateChangeConfirmation(newValue.format("DD MMM YYYY"))));
    if (result.action != DialogAction.yes) {
      batch.expiryDateLocal = moment.utc(batch.expiryDateUtc).local();
      return;
    }    


    try {
      this._loaderService.show();
      await firstValueFrom(this._api.send(new UpdateAccessCodeBatchExpiryDateCommand(batch.batchId, batch.expiryDateLocal?.utc().toJSON() ?? '')));
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
}
