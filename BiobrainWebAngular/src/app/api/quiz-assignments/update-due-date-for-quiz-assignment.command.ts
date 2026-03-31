import moment, { Moment } from 'moment';

import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class UpdateDueDateForQuizAssignmentCommand extends Command<EmptyCommandResult> {  

  constructor(
    public readonly quizAssignmentId: string,
    public readonly dueDateLocal: Moment,
    public readonly dueDateUtc: Moment) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/UpdateDueDateForQuizAssignment`;
  }
}
