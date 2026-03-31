import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class DeleteContentTreeNodeCommand extends Command<DeleteContentTreeNodeCommand_Result> {
  constructor(
    public readonly nodeId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/DeleteContentTreeNode`;
  }
}

export class DeleteContentTreeNodeCommand_Result {
}
