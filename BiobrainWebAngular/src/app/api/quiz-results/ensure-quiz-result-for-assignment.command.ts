import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class EnsureQuizResultForAssignmentCommand extends Command<EnsureQuizResultForAssignmentCommand_Result> {
  constructor(
    public readonly userId: string,
    public readonly quizStudentAssignmentId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/EnsureQuizResultForAssignment`;
  }
}

export interface EnsureQuizResultForAssignmentCommand_Result {
  quizResultId: string;
}
