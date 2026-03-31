import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class DeleteUserGuideNodeCommand extends Command<EmptyCommandResult> {
  constructor(public readonly nodeId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userGuide()}/DeleteUserGuideNode`;
  }
}
