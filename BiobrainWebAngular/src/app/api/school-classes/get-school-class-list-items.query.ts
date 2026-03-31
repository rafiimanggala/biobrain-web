import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetSchoolClassListItemsQuery extends Query<GetSchoolClassListItemsQuery_Result[]> {
  constructor(public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/GetAsListItems`;
  }
}


export interface GetSchoolClassListItemsQuery_Result {
  schoolClassId: string;
  name: string;
}
