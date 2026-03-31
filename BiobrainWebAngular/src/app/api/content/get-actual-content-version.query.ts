import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetActualContentVersionQuery extends Query<GetActualContentVersionQuery_Result[]> {

  constructor(public userId: string){super();}

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetActualContentVersion`;
  }
}

export interface GetActualContentVersionQuery_Result {
  version: number;
  courseId: string;
  courseName: string;
}
