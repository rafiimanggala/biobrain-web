import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetStudentListItemsQuery extends Query<GetStudentListItemsQuery_Result[]> {
  constructor(public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/GetAsListItems`;
  }
}


export interface GetStudentListItemsQuery_Result {
  studentId: string;
  fullName: string;
}
