import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import moment from 'moment';
import { Observable } from 'rxjs';
import { DeleteAccountCommand } from 'src/app/api/accounts/delete-account.command';
import { Api } from 'src/app/api/api.service';
import { PaymentPeriod } from 'src/app/api/enums/payment-period.enum';
import { SubscriptionStatus } from 'src/app/api/enums/subscription-status.enum';
import { CancelSubjectsCommand } from 'src/app/api/payments/cancel-subjects.comnand';
import { GetSubscriptionsListQuery, GetSubscriptionsListQuery_Result } from 'src/app/api/payments/get-subscription-list.query';
import { LogoutOperation } from 'src/app/auth/operations/logout.operation';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { BadRequestCommonException } from 'src/app/core/exceptions/bad-request-common.exception';
import { InternalServerException } from 'src/app/core/exceptions/internal-server.exception';
import { RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';
import { AddSubscriptionOperation } from 'src/app/payment/operations/add-subscription.operation';
import { EditPaymentDetailsOperation } from 'src/app/payment/operations/edit-payment-details.operation';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';
import { CancelSubjectsDialog } from '../cancel-subjects/cancel-subjects.dialog';
import { CancelSubscriptionDialog } from '../cancel-subscription/cancel-subscription.dialog';
import { SubscriptionListDialogData } from './subscription-list-dialog-data';

@Component({
  selector: 'subscription-list-dialog',
  templateUrl: 'subscription-list.dialog.html',
  styleUrls: ['subscription-list.dialog.scss'],
})
export class SubscriptionListDialog extends DialogComponent<SubscriptionListDialogData> {

  contentSubscriptions: Observable<GetSubscriptionsListQuery_Result[]>;

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
    private readonly _appEvents: AppEventProvider,
    private readonly _dialog: Dialog,
    private readonly _editPaymentDetails: EditPaymentDetailsOperation,
    private readonly _addSubscriptionOperation: AddSubscriptionOperation,
    private readonly _logoutOperation: LogoutOperation,
    @Inject(MAT_DIALOG_DATA) public dialogData: SubscriptionListDialogData,
  ) {
    super(dialogData);
    this.contentSubscriptions = this._api.send(new GetSubscriptionsListQuery(this.dialogData.userId));
  }

  getDateStringFromUtc(dateTime: string): string{
    var converted = moment.utc(dateTime).local();
    return converted.format("DD-MMM-YYYY");
  }

  getTimeStringFromUtc(dateTime: string): string{
    var converted = moment.utc(dateTime).local();
    return converted.format("HH:mm");
  }

  getStatusText(status: SubscriptionStatus): string{
    switch(status){
      case SubscriptionStatus.Success: return this.strings.active;
      case SubscriptionStatus.StoppedByUser: return this.strings.renewalCanceled;
      case SubscriptionStatus.PaymentFailed: return this.strings.paymentFailed;
      default: return SubscriptionStatus[status];
    }
  }

  getPeriodText(period: PaymentPeriod): string{
    switch(period){
      case PaymentPeriod.Monthly: return this.strings.monthly;
      case PaymentPeriod.Yearly: return this.strings.annually;
    }
  }

  async onCancelSubscription(subscription: GetSubscriptionsListQuery_Result){
    if(subscription.status == SubscriptionStatus.StoppedByUser || subscription.status == SubscriptionStatus.Inactive) return;
    var confirmation = await this._dialog.show(CancelSubjectsDialog, subscription.courses, {panelClass: "bordered-dialog-panel"});
    if (confirmation.action !== DialogAction.save || !confirmation.data) return;

    try {
      this._loaderService.show();
      await this._api.send(new CancelSubjectsCommand(this.dialogData.userId, subscription.scheduledPaymentId, confirmation.data)).toPromise();
    }
    catch (e) {
      if (e instanceof BadRequestCommonException) this._appEvents.errorEmit(e.message);
      if (e instanceof InternalServerException) this._appEvents.errorEmit(e.message);
      if (e instanceof RequestValidationException) this._appEvents.errorEmit(validationExceptionToString(e));
    }
    finally{      
      this._loaderService.hideIfVisible();
    }

    await this._dialog.show(CancelSubscriptionDialog, this.dialogData.userName, {panelClass: "bordered-dialog-panel"});
    await this.refreshData();

  }

  async deleteAccount(){    
    var confirmation = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this.strings.deleteAccount, this.strings.deleteAccountConfirmation));
    if (confirmation.action !== DialogAction.yes) return;

    try {
      this._loaderService.show();
      await this._api.send(new DeleteAccountCommand(this.dialogData.userId)).toPromise();
    }
    catch (e) {
      if (e instanceof BadRequestCommonException) this._appEvents.errorEmit(e.message);
      if (e instanceof InternalServerException) this._appEvents.errorEmit(e.message);
      if (e instanceof RequestValidationException) this._appEvents.errorEmit(validationExceptionToString(e));
    }
    finally{      
      this._loaderService.hideIfVisible();
    }  

    await this._logoutOperation.perform();
    this.close(DialogAction.cancel);
  }

  async onAddSubscription(){
    await this._addSubscriptionOperation.perform();
    await this.refreshData();
  }

  async onEditBillingDetails(){
    await this._editPaymentDetails.perform();
  }

  private async refreshData(){
    this.contentSubscriptions = this._api.send(new GetSubscriptionsListQuery(this.dialogData.userId));
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }
}
