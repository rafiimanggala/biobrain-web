import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GeneratePracticeSetCommand extends Command<GeneratePracticeSetCommand_Result> {
  constructor(
    public readonly courseId: string,
    public readonly contentTreeNodeId: string,
    public readonly questionCount: number,
    public readonly questionType: string,
    public readonly teacherId: string,
    public readonly difficultyLevel: string = 'Medium'
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.ai()}/generate-practice-set`;
  }
}

export interface GeneratePracticeSetCommand_Result {
  questionIds: string[];
}
