import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class SaveUserGuideContentCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly articleId: string | null,
    public readonly nodeId: string,
    public readonly htmlText: string,
    public readonly videoUrl: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userGuide()}/SaveUserGuideContent`;
  }
}
