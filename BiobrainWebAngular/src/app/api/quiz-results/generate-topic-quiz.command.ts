import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GenerateTopicQuizCommand extends Command<GenerateTopicQuizCommand_Result> {
  constructor(
    public readonly contentTreeNodeId: string,
    public readonly questionCount: number,
    public readonly userId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/GenerateTopicQuiz`;
  }
}

export interface GenerateTopicQuizCommand_Result {
  quizStudentAssignmentId: string;
}
