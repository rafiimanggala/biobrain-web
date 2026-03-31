import { PaymentConfirmationData } from "../../components/payment-confirmation/payment-confirmation-data";
import { SubscriptionData } from "../../components/subscription-details.component.ts/subscription.data";

export class SubscriptionDialogResult{
    constructor(
        public promocode: PaymentConfirmationData,
        public subscriptionData: SubscriptionData
    ){}
}