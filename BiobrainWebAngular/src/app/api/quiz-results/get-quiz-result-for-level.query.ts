import { Moment } from 'moment';
import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';
import { Command } from '../common/command';

export class GetQuizResultForLevelQuery extends Command<GetQuizResultForLevelQuery_Result> {
  constructor(public readonly courseId: string, public readonly levelIds: string[]) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/GetQuizResultForLevel`;
  }
}

export interface GetQuizResultForLevelQuery_ResultQuizResults {
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

export interface GetQuizResultForLevelQuery_Result {
  quizResults: GetQuizResultForLevelQuery_ResultQuizResults[];
}
