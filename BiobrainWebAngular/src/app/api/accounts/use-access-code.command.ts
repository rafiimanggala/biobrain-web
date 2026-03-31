import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class UseAccessCodeCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly studentId: string,
    public readonly accessCode: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/UseAccessCode`;
  }
}
