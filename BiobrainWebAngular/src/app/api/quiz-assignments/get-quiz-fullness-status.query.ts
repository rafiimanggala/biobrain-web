import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetQuizFullnessStatusQuery extends Query<GetQuizFullnessStatusQuery_Result> {
  constructor(public readonly quizId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizzesAnalytic()}/GetQuizFullnessStatus`;
  }
}

export interface GetQuizFullnessStatusQuery_Result {
  readonly isQuizFull: boolean;
}

