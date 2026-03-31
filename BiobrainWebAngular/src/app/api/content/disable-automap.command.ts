import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class DisableAutoMapCommand extends Command<EmptyCommandResult> {
  constructor(
    public quizId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/DisableAutoMap`;
  }
}
