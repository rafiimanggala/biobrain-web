import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetSubjectsQuery extends Query<GetSubjectsQuery_Result[]> {
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.subjects()}/GetSubjects`;
  }
}

export interface GetSubjectsQuery_Result {
  subjectCode: number;
  name: string;
}
