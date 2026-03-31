import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class ResetSelfPasswordCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly loginName: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/ResetSelfPassword`;
  }
}
