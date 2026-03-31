import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';
import { PaymentPeriod } from '../enums/payment-period.enum';

export class GetSubscriptionParametersQuery extends Query<GetSubscriptionParametersQuery_Result> {
  constructor(public readonly studentId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/GetSubscriptionParameters`;
  }
}

export interface GetSubscriptionParametersQuery_Result {
  // products: GetSubscriptionParametersQuery_ResultProduct[];
  // additionalProducts: GetSubscriptionParametersQuery_ResultProduct[];
  prices: GetSubscriptionParametersQuery_ResultPrice[];
  subjects: GetSubscriptionParametersQuery_ResultSubject[];
  additionalSubjects: GetSubscriptionParametersQuery_ResultSubject[];
  curricula: GetSubscriptionParametersQuery_ResultCurriculum[];
  currency: string;
  userCurriculumCode?: number;  
  selectedPaymentPeriod?: PaymentPeriod;
  country: string;
  voucherAmount: number|null;
  voucherId: string|null;
}

export interface GetSubscriptionParametersQuery_ResultCurriculum {
  curriculumCode: number;
  name: string;
  isGeneric: boolean;
  years: number[];
}

export interface GetSubscriptionParametersQuery_ResultProduct {
  productId: string;
  name: string;
  curriculumCode: number;
  year: number;
  subjectCode: number;
  isComingSoon: boolean;
}

export interface GetSubscriptionParametersQuery_ResultSubject {
  subjectCode: number;
  name: string;
  isSelected: boolean;
  // year?: number;
  isNeedYearSelection: boolean;
  curriculumCode: number;
  products: GetSubscriptionParametersQuery_ResultProduct[]
  selectedProduct: GetSubscriptionParametersQuery_ResultProduct;
}

export interface GetSubscriptionParametersQuery_ResultPrice {
  subjectsNumber: number;
  period: PaymentPeriod;
  value: number;
  isDisplayed: boolean;
}
