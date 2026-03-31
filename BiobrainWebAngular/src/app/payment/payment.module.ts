import { NgModule } from '@angular/core';

import { MaterialModule } from '../share/material.module';
import { SharedModule } from '../share/shared.module';
import { CoreModule } from '@angular/flex-layout';
import { CommonModule } from '@angular/common';
import { ApiModule } from '../api/api.module';
import { PaymentDetailsComponent } from './views/components/payment-details/payment-details.component';
import { SubscriptionComponent } from './views/pages/subscription/subscription.component';
import { PaymentRoutingModule } from './payment-routing.module';
import { SubscriptionDetailsComponent } from './views/components/subscription-details.component.ts/subscription-details.component';
import { SubscriptionDialog } from './views/dialogs/subscription/subscription.dialog';
import { SubscriptionsListOperation } from './operations/edit-subscription.operation';
import { PaymentDetailsDialog } from './views/dialogs/payment-details/payment-details.dialog';
import { SubscriptionListDialog } from './views/dialogs/subscription-list/subscription-list.dialog';
import { EditPaymentDetailsOperation } from './operations/edit-payment-details.operation';
import { AddSubscriptionOperation } from './operations/add-subscription.operation';
import { CancelSubscriptionDialog } from './views/dialogs/cancel-subscription/cancel-subscription.dialog';
import { CancelSubjectsDialog } from './views/dialogs/cancel-subjects/cancel-subjects.dialog';
import { PaymentConfirmationComponent } from './views/components/payment-confirmation/payment-confirmation.component';
import { PaymentStringsService } from './services/payment-strings.service';

const components: any[] = [
  PaymentDetailsComponent,
  SubscriptionComponent,
  SubscriptionDetailsComponent,
  PaymentConfirmationComponent,
];

const dialogs: any[] = [SubscriptionDialog, PaymentDetailsDialog, SubscriptionListDialog, CancelSubscriptionDialog, CancelSubjectsDialog];

const operations: any[] = [SubscriptionsListOperation, EditPaymentDetailsOperation, AddSubscriptionOperation];

const services: any[] = [PaymentStringsService];

@NgModule({
  declarations: [
    components,
    dialogs,
  ],
  providers: [
    operations,
    services,
  ],
  entryComponents: [dialogs],
  imports: [
    CoreModule,
    CommonModule,
    SharedModule,
    ApiModule,
    MaterialModule,
    PaymentRoutingModule,
  ],
})
export class PaymentModule {
}
