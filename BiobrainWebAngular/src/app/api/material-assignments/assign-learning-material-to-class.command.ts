import moment, { Moment } from 'moment';

import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AssignLearningMaterialToClassCommand extends Command<AssignLearningMaterialToClassCommand_Result> {
  public readonly dueDateLocal: Moment;
  public readonly assignedDateLocal: Moment;
  public readonly assignedDateUtc: Moment;

  constructor(
    public readonly studentIdsBySchoolClassIdMap: Record<string, string[]>,
    public readonly contentTreeNodeIds: string[],
    public readonly dueDateUtc: Moment,
    public readonly forceCreateNew: boolean = false) {
    super();

    this.dueDateLocal = dueDateUtc.clone().utc(true);
    this.assignedDateUtc = moment();
    this.assignedDateLocal = this.assignedDateUtc.clone().utc(true);
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.learningMaterialAssignments()}/AssignLearningMaterialToClass`;
  }
}

export interface AssignLearningMaterialToClassCommand_Result {
  notAssignedNodeIds: string[];
}


