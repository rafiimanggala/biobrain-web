import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetPaymentMethodsQuery extends Query<GetPaymentMethodsQuery_Result[]> {
  constructor() {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/GetPaymentMethods`;
  }
}

export interface GetPaymentMethodsQuery_Result {
  name:string;
  publicKey: string;
  apiBaseUrl: string;
}
