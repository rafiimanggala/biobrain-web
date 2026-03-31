import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CancelScheduledPaymentCommand extends Command<CancelScheduledPaymentCommand_Result> {
  constructor(
    public userId: string,
    public subscriptionId: string,
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/CancelScheduledPayment`;
  }
}

export interface CancelScheduledPaymentCommand_Result {
}
