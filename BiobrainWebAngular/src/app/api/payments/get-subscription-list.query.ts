import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';
import { PaymentPeriod } from '../enums/payment-period.enum';
import { SubscriptionStatus } from '../enums/subscription-status.enum';
import { SubscriptionSubjectStatus } from '../enums/subscription-subject-status.enum';

export class GetSubscriptionsListQuery extends Query<GetSubscriptionsListQuery_Result[]> {
  constructor(public readonly userId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/GetScheduledPayments`;
  }
}

export interface GetSubscriptionsListQuery_Result {
  scheduledPaymentId: string,
  period: PaymentPeriod,
  status: SubscriptionStatus,
  amount: number,
  currency: string,
  nextPayDateUtc: string,
  courses: GetSubscriptionsListQuery_Result_Course[]
}

export interface GetSubscriptionsListQuery_Result_Course{
  courseId: string;
  courseName: string;
  status: SubscriptionSubjectStatus;
}
