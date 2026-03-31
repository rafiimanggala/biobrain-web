import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class DeleteVoucherCommand extends Command<EmptyCommandResult> {
  constructor(  
    public voucherId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.vouchers()}/DeleteVoucher`;
  }

  
}

