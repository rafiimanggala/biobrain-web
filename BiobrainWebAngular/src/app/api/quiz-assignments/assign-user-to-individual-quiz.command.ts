import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AssignUserToIndividualQuizCommand extends Command<AssignUserToIndividualQuizCommand_Result> {
  constructor(
    public readonly userId: string,
    public readonly quizId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/AssignUserToIndividualQuiz`;
  }
}

export interface AssignUserToIndividualQuizCommand_Result {
  quizResultId: string;
}
