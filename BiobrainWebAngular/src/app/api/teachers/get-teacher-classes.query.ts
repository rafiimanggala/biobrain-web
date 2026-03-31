import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetTeacherClassesQuery extends Query<GetTeacherClassesQuery_Result[]> {
  constructor(public readonly teacherId: string, public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/GetTeacherClasses`;
  }
}

export interface GetTeacherClassesQuery_Result {
  schoolClassId: string;
  schoolClassYear: number;
  schoolClassName: string;
  courseId: string;
  subjectCode: number;
  curriculumCode: number;
}
