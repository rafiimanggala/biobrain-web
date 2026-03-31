import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GetCardTokenQuery extends Command<GetCardTokenQuery_Result> {
  constructor(
    public cardNumber: string,
    public expiryMonth: number,
    public expiryYear: number,
    public cvc: string,
    public cardholderName: string,
    public addressLine1: string,
    public city: string,
    public country: string,
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/GetCardToken`;
  }
}

export interface GetCardTokenQuery_Result {
  cardToken: string,
}
