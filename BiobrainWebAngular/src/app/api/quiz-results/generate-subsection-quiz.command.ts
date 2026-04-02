import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GenerateSubsectionQuizCommand extends Command<GenerateSubsectionQuizCommand_Result> {
  constructor(
    public readonly contentTreeNodeId: string,
    public readonly questionCount: number,
    public readonly userId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/GenerateSubsectionQuiz`;
  }
}

export interface GenerateSubsectionQuizCommand_Result {
  quizStudentAssignmentId: string;
  questionCount: number;
}
