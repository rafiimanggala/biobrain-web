import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class IncludeQuestionToAutoMapCommand extends Command<EmptyCommandResult> {
  constructor(
    public quizId: string,
    public questionId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/IncludeQuestionToQuizAutoMap`;
  }
}
