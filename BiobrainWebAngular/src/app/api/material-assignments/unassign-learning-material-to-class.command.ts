import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class UnassignLearningMaterialToClassCommand extends Command<EmptyCommandResult> {  

  constructor(
    public readonly learningMaterialAssignmentId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.learningMaterialAssignments()}/UnassignLearningMaterialToClass`;
  }
}
