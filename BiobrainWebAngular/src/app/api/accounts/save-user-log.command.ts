import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class SaveUserLogCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly log: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/SaveUserLog`;
  }
}