import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { PaymentPeriod } from '../enums/payment-period.enum';

export class AddScheduledPaymentCommand extends Command<AddScheduledPaymentCommand_Result> {
  constructor(
    public userId: string,
    public courses: string[],
    public period: PaymentPeriod,
    public promoCodeId: string|null
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/AddScheduledPayment`;
  }
}

export interface AddScheduledPaymentCommand_Result {
}
