import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CancelSubjectsCommand extends Command<CancelSubjectsCommand_Result> {
  constructor(
    public userId: string,
    public subscriptionId: string,
    public courseIds: string[]
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/CancelScheduledPaymentSubjects`;
  }
}

export interface CancelSubjectsCommand_Result {
}
