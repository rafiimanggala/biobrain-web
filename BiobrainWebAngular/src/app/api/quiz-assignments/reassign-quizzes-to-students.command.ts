import moment, { Moment } from 'moment';

import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class ReassignQuizzesToStudentsCommand extends Command<ReassignQuizzesToStudentsCommand_Result> {
  public readonly dueDateLocal: Moment;
  public readonly assignedDateLocal: Moment;
  public readonly assignedDateUtc: Moment;

  constructor(
    public quizAssignmentIds: string[],
    public studentIds: string[],
    public dueDateUtc: Moment
  ) {
    super();

    this.dueDateLocal = dueDateUtc.clone().utc(true);
    this.assignedDateUtc = moment();
    this.assignedDateLocal = this.assignedDateUtc.clone().utc(true);
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/ReassignQuizzesToStudents`;
  }
}

export interface ReassignQuizzesToStudentsCommand_Result {
  quizStudentAssignmentIds: string[];
}
