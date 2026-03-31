import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetCoursesForSchoolQuery extends Query<GetCoursesForSchoolQuery_Result[]> {
  constructor(public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.courses()}/GetCoursesForSchool`;
  }
}

export interface GetCoursesForSchoolQuery_Result {
  courseId: string;
  subjectCode: number;
  curriculumCode: number;
}
