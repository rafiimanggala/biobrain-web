import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class DeleteAccountPemanentCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly userId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/DeleteUserPermanent`;
  }
}