import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class SaveUserGuideNodeCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly nodeId: string | null,
    public readonly parentId: string | null,
    public readonly name: string,
    public readonly isAvailableForStudent: boolean
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userGuide()}/SaveUserGuideNode`;
  }
}
