import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AddTrialScheduledPaymentCommand extends Command<AddTrialScheduledPaymentCommand_Result> {
  constructor(
    public userId: string,
    public courses: string[],
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/AddTrialScheduledPayment`;
  }
}

export interface AddTrialScheduledPaymentCommand_Result {
}
