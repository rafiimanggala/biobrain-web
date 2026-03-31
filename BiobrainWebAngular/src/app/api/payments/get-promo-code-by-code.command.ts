import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { PaymentPeriod } from '../enums/payment-period.enum';

export class GetPromoCodeByCodeCommand extends Command<GetPromoCodeByCodeCommand_Result> {
  constructor(
    public promoCode: string,
    public paymentPeriod: PaymentPeriod,
    public bundleSize: number,
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/GetPromoCodeByCode`;
  }
}

export interface GetPromoCodeByCodeCommand_Result {
  promoCodeId: string;
  code: string;
  amount: number|null;
  percent: number|null;
}
