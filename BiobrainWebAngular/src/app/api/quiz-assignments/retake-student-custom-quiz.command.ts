import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class RetakeStudentCustomQuizCommand extends Command<RetakeStudentCustomQuizCommand_Result> {
  constructor(
    public readonly quizId: string,
    public readonly userId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/RetakeStudentCustomQuiz`;
  }
}

export interface RetakeStudentCustomQuizCommand_Result {
  quizStudentAssignmentId: string;
}
