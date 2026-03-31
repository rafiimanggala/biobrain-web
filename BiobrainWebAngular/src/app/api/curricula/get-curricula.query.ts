import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetCurriculaQuery extends Query<GetCurriculaQuery_Result[]> {
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.curricula()}/GetCurricula`;
  }
}

export interface GetCurriculaQuery_Result {
  curriculumCode: number;
  name: string;
}
