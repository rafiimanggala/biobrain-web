import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class ChangeSelfEmailCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly userId: string,
    public readonly email: string,
    public readonly password: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/ChangeSelfEmail`;
  }
}
