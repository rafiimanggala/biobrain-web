import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetLastIndividualUncompletedQuizResultQuery extends Query<GetLastIndividualUncompletedQuizResultQuery_Result> {
  constructor(
    public readonly userId: string,
    public readonly quizId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/GetLastIndividualUncompletedQuizResult`;
  }
}

export interface GetLastIndividualUncompletedQuizResultQuery_Result {
  quizResultId: string | null | undefined;
}
