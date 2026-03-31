import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { PaymentPeriod } from '../enums/payment-period.enum';

export class SaveScheduledPaymentCommand extends Command<AddSubscriptionCommand_Result> {
  constructor(
    public userId: string,
    public cardToken?: string,
    public courses?: string[],
    public period?: PaymentPeriod,
    public promoCodeId?: string|null
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/SaveScheduledPaymentAndCard`;
  }
}

export interface AddSubscriptionCommand_Result {
}
