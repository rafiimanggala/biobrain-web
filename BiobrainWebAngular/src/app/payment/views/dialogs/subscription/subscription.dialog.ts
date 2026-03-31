import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GetSubscriptionParametersQuery_Result } from 'src/app/api/payments/get-subscription-parameters.query';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { StringsService } from 'src/app/share/strings.service';
import { PaymentConfirmationData } from '../../components/payment-confirmation/payment-confirmation-data';
import { SubscriptionData } from '../../components/subscription-details.component.ts/subscription.data';
import { SubscriptionDialogResult } from './subscription-dialog-result';

export enum SubscriptionDetailsMode{
  SubscriptionData = 1,
  Confirmation = 2
}

@Component({
  selector: 'subscription-dialog',
  templateUrl: 'subscription.dialog.html',
  styleUrls: ['subscription.dialog.scss'],
})
export class SubscriptionDialog extends DialogComponent<GetSubscriptionParametersQuery_Result, SubscriptionDialogResult> {

  public mode: SubscriptionDetailsMode = SubscriptionDetailsMode.SubscriptionData;
  public subscriptionData!: SubscriptionData;

  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: GetSubscriptionParametersQuery_Result,
  ) {
    super(dialogData);
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }

  onNext(data: SubscriptionData){
    this.subscriptionData = data;
    this.mode = 2;
  }

  onSubmitSubscription(data: PaymentConfirmationData): void {
    this.close(DialogAction.save, new SubscriptionDialogResult(data, this.subscriptionData));
  }

  onEditBillingDetails(){
    this.close(DialogAction.next);
  }

  onCancelSubscription(){
    this.close(DialogAction.delete);
  }
}
