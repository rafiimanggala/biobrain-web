import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class ChangeSelfPasswordCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly userId: string,
    public readonly oldPassword: string,
    public readonly newPassword: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/ChangeSelfPassword`;
  }
}
