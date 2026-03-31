import { Moment } from 'moment';
import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetQuizResultHistoryQuery extends Query<GetQuizResultHistoryQuery_Result> {
  constructor(public readonly courseId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/GetQuizResultHistory`;
  }
}

export interface GetQuizResultHistoryQuery_ResultQuizResults {
  quizResultId: string;
  quizId: string;
  courseId: string;
  unitId: string;
  nodeId: string;
  parentNodeId: string | null;
  path: string[];
  nameLines: string[],
  score: number;
  date: string;
}

export interface GetQuizResultHistoryQuery_Result {
  averageQuizRate: number;
  quizzesCompletedRate: number;
  value: string;
  quizResults: GetQuizResultHistoryQuery_ResultQuizResults[];
}
