import { PaymentPeriod } from 'src/app/api/enums/payment-period.enum';

export class SubscriptionData {
  constructor(
    public courseIds: string[],
    public period: PaymentPeriod,
    public total: number,
    public curency: string,
    public curriculumCode: number,
    public country: string
  ) {
  }
}
