import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class SetPasswordCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly loginName: string,
    public readonly token: string,
    public readonly newPassword: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/SetPassword`;
  }
}
