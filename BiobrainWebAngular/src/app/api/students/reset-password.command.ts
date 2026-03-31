import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class ResetPasswordCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly userId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/ResetPassword`;
  }
}
