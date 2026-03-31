import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateQuizFromTemplateCommand extends Command<CreateQuizFromTemplateCommand_Result> {
  constructor(
    public readonly templateId: string,
    public readonly schoolClassId: string,
    public readonly teacherId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/CreateQuizFromTemplate`;
  }
}

export interface CreateQuizFromTemplateCommand_Result {
  readonly quizAssignmentId: string;
}
