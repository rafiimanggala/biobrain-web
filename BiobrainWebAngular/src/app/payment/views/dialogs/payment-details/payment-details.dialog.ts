import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GetPaymentMethodsQuery_Result } from 'src/app/api/payments/get-pyment-methods.query';
import { GetSubscriptionParametersQuery_Result } from 'src/app/api/payments/get-subscription-parameters.query';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { DialogComponent } from 'src/app/core/dialogs/dialog-component';
import { StringsService } from 'src/app/share/strings.service';
import { PaymentDetailsData } from '../../components/payment-details/payment-details-data';
import { SubscriptionData } from '../../components/subscription-details.component.ts/subscription.data';

@Component({
  selector: 'payment-details-dialog',
  templateUrl: 'payment-details.dialog.html',
  styleUrls: ['payment-details.dialog.scss'],
})
export class PaymentDetailsDialog extends DialogComponent<GetPaymentMethodsQuery_Result[], PaymentDetailsData> {
  constructor(
    public readonly strings: StringsService,
    @Inject(MAT_DIALOG_DATA) public dialogData: GetPaymentMethodsQuery_Result[],
  ) {
    super(dialogData);
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }

  onSubmit(data: PaymentDetailsData): void {
    this.close(DialogAction.save, data);
  }
}
