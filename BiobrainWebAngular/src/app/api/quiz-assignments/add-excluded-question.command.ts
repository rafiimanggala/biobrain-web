import moment, { Moment } from 'moment';

import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AddExcludedQuestionCommand extends Command<AddExcludedQuestionCommand_Result> {

  constructor(
    public schoolClassId: string,
    public quizId: string,
    public questionId: string,
    public isExcluded: boolean
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/AddExcludedQuestion`;
  }
}


export interface AddExcludedQuestionCommand_Result {
}
