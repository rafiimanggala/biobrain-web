import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetVouchersQuery extends Query<GetVouchersQuery_Result[]> {
  constructor(
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.vouchers()}/GetVouchers`;
  }
}

export interface GetVouchersQuery_Result {
  voucherId: string;
  code: string;
  note: string;
  totalAmount: number;
  amountUsed: number;
  country: string;
  expiryDateUtc: string;
  redeemExpiryDateUtc: string;
  createdAt: string;
}