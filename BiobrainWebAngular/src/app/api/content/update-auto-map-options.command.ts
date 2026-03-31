import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class UpdateAutoMapOptionsCommand extends Command<EmptyCommandResult> {
  constructor(
    public quizId: string,
    public baseQuizId: string,
    public parentNodeId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/UpdateAutoMapOptions`;
  }
}
