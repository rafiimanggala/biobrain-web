import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SetAssignedLearningMaterialAsDoneCommand extends Command<SetAssignedLearningMaterialAsDoneCommand_Result> {
  constructor(public readonly learningMaterialUserAssignmentId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.learningMaterialAssignments()}/SetAssignedLearningMaterialAsDone`;
  }
}

export interface SetAssignedLearningMaterialAsDoneCommand_Result {
  readonly completedAtUtc: string | undefined;
  readonly learningMaterialUserAssignmentId: string;
}
