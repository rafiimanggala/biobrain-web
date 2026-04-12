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
    public readonly studentIds: string[] = [],
    public readonly dueDateUtc: string | null = null,
    public readonly dueDateLocal: string | null = null,
    public readonly hintsEnabled: boolean = true,
    public readonly soundEnabled: boolean = true,
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
