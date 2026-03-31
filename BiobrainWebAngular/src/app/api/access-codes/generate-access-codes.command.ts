import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GenerateAccessCodesCommand extends Command<GenerateAccessCodesCommand_Result> {
  constructor(  
    public note: string,
    public courseIds: string[],
    public numberOfCodes: number,
    public expiryDate: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accessCodes()}/GenerateAccessCodes`;
  }

  
}

export interface GenerateAccessCodesCommand_Result{
  batchId: string;
  batchHeader: string;
  codes: string[];
  createdAtUtc: string;
}

