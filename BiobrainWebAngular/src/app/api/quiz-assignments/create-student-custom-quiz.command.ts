import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateStudentCustomQuizCommand extends Command<CreateStudentCustomQuizCommand_Result> {
  constructor(
    public readonly name: string,
    public readonly courseId: string,
    public readonly contentTreeNodeIds: string[],
    public readonly questionCount: number,
    public readonly userId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/CreateStudentCustomQuiz`;
  }
}

export interface CreateStudentCustomQuizCommand_Result {
  quizStudentAssignmentId: string;
}
