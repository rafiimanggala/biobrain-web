import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetSchoolClassesListQuery extends Query<GetSchoolClassesListQuery_Result[]> {
  constructor(public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/GetSchoolClasses`;
  }
}

export interface GetSchoolClassesListQuery_Result {
  schoolClassId: string;
  schoolId: string;
  courseId: string;
  year: number;
  name: string;
  autoJoinClassCode: string | null | undefined;
  teachers: string[];
}
