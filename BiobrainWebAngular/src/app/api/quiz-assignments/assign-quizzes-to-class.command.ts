import moment, { Moment } from 'moment';

import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AssignQuizzesToClassCommand extends Command<AssignQuizzesToClassCommand_Result> {
  public readonly dueDateLocal: Moment;
  public readonly assignedDateLocal: Moment;
  public readonly assignedDateUtc: Moment;

  constructor(
    public studentIdsBySchoolClassIdMap: Record<string, string[]>,
    public quizIds: string[],
    public dueDateUtc: Moment,
    public readonly forceCreateNew: boolean = false,
    public readonly hintsEnabled: boolean = true,
    public readonly soundEnabled: boolean = true
  ) {
    super();

    this.dueDateLocal = dueDateUtc.clone().utc(true);
    this.assignedDateUtc = moment();
    this.assignedDateLocal = this.assignedDateUtc.clone().utc(true);
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/AssignQuizzesToClass`;
  }
}


export interface AssignQuizzesToClassCommand_Result {
  quizAssignmentIds: string[];
  notAssignedQuizIds: string[];
}
