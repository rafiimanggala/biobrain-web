import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetTeacherListItemsQuery extends Query<GetTeacherListItemsQuery_Result[]> {
  constructor(
    public readonly schoolId: string,
    public readonly searchText: string | undefined | null
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/GetAsListItems`;
  }
}


export interface GetTeacherListItemsQuery_Result {
  teacherId: string;
  fullName: string;
}
