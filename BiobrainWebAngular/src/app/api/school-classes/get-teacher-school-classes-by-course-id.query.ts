import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetTeacherSchoolClassesByCourseIdQuery extends Query<GetSchoolClassByCourseIdQuery_Result[]> {
  constructor(
    public readonly courseId: string,
    public readonly schoolId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/GetForTeacherByCourseId`;
  }
}

export interface GetSchoolClassByCourseIdQuery_Result {
  schoolClassId: string;
  schoolId: string;
  courseId: string;
  year: number;
  name: string;
  autoJoinClassCode: string;
}
