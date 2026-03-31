import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetAutoMapOptionsQuery extends Query<GetAutoMapOptionsQuery_Result> {

  constructor(public quizId: string){super();}

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetAutoMapOptions`;
  }
}

export interface GetAutoMapOptionsQuery_Result {
  baseQuizId: string;
  baseCourseId: string;
}
