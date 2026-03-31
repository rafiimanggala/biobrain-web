import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AddVoucherScheduledPaymentCommand extends Command<AddVoucherScheduledPaymentCommand_Result> {
  constructor(
    public userId: string,
    public courses: string[],
    public voucherId: string
    ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.payment()}/AddVoucherScheduledPayment`;
  }
}

export interface AddVoucherScheduledPaymentCommand_Result {
}
