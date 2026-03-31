import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class SendQuestionCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly text: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userGuide()}/SendQuestion`;
  }
}
