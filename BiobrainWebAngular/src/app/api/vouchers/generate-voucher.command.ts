import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GenerateVoucherCommand extends Command<GenerateVoucherCommand_Result> {
  constructor(  
    public note: string,
    public totalAmount: number,
    public country: string,
    public expiryDateUtc: string,
    public redeemExpiryDateUtc: string,
    public numberOfVouchers: number,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.vouchers()}/CreateVoucher`;
  }

  
}

export interface GenerateVoucherCommand_Result{
}

