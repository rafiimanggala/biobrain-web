import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class ReorderUserGuideNodeCommand extends Command<EmptyCommandResult> {
  constructor(public readonly nodeId: string, public readonly newOrder: number) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userGuide()}/ReorderUserGuideContentTreeNode`;
  }
}
