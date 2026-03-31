import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class UnassignQuizToClassCommand extends Command<EmptyCommandResult> {  

  constructor(
    public readonly quizAssignmentId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/UnassignQuizzToClass`;
  }
}
