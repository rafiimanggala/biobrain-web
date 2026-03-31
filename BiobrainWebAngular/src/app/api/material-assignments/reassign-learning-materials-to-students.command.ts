import moment, { Moment } from 'moment';

import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class ReassignLearningMaterialsToStudentsCommand extends Command<EmptyCommandResult> {
  public readonly dueDateLocal: Moment;
  public readonly assignedDateLocal: Moment;
  public readonly assignedDateUtc: Moment;

  constructor(
    public readonly schoolClassId: string,
    public contentTreeNodeIds: string[],
    public studentIds: string[],
    public dueDateUtc: Moment
  ) {
    super();

    this.dueDateLocal = dueDateUtc.clone().utc(true);
    this.assignedDateUtc = moment();
    this.assignedDateLocal = this.assignedDateUtc.clone().utc(true);
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.learningMaterialAssignments()}/ReassignLearningMaterialsToStudents`;
  }
}
