import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class DeleteAccessCodesCommand extends Command<EmptyCommandResult> {
  constructor(  
    public accessCodeBatchId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accessCodes()}/DeleteAccessCodes`;
  }

  
}

