import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateAccessCodeBatchExpiryDateCommand extends Command<UpdateAccessCodeBatchExpiryDateCommand_Result> {
  constructor(  
    public batchId: string,
    public expiryDate: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accessCodes()}/UpdateAccessCodeBatchExpiryDate`;
  }

  
}

export interface UpdateAccessCodeBatchExpiryDateCommand_Result{
  batchId: string;
  batchHeader: string;
  codes: string[];
  createdAtUtc: string;
}

