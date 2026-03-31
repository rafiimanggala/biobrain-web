import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdatePaymentDetailsCommand extends Command<UpdatePaymentDetailsCommand_Result> {
  constructor(
    public userId: string,
    public cardToken?: string,
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/UpdatePaymentDetails`;
  }
}

export interface UpdatePaymentDetailsCommand_Result {
}
