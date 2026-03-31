import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetSchoolListItemsQuery extends Query<GetSchoolListItemsQuery_Result[]> {
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/GetAsListItems`;
  }
}

export interface GetSchoolListItemsQuery_Result {
  schoolId: string;
  name: string;
}
