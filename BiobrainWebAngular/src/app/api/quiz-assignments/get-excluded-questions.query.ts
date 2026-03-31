import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { Query } from '../common/query';

export class GetExcludedQuestionsQuery extends Query<GetExcludedQuestionsQuery_Result> {
  constructor(public readonly schoolClassId: string, public readonly quizId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/GetExcludedQuestions`;
  }
}

export interface GetExcludedQuestionsQuery_Result {
  readonly questionIds: string[];
}

