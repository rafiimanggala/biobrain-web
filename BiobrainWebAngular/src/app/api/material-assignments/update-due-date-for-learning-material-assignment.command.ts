import moment, { Moment } from 'moment';

import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class UpdateDueDateForLearningMaterialAssignmentCommand extends Command<EmptyCommandResult> {  

  constructor(
    public readonly learningMaterialAssignmentId: string,
    public readonly dueDateLocal: Moment,
    public readonly dueDateUtc: Moment) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.learningMaterialAssignments()}/UpdateDueDateForLearningMaterialAssignment`;
  }
}
