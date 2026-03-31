import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateTeacherCustomQuizCommand extends Command<CreateTeacherCustomQuizCommand_Result> {
  constructor(
    public readonly name: string,
    public readonly courseId: string,
    public readonly contentTreeNodeIds: string[],
    public readonly questionCount: number,
    public readonly schoolClassId: string,
    public readonly saveAsTemplate: boolean,
    public readonly teacherId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/CreateTeacherCustomQuiz`;
  }
}

export interface CreateTeacherCustomQuizCommand_Result {
  readonly quizAssignmentId: string;
}
