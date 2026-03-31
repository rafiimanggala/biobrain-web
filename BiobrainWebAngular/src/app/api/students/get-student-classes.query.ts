import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetStudentClassesQuery extends Query<GetStudentClassesQuery_Result[]> {
  constructor(public readonly studentId: string, public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/GetStudentClasses`;
  }
}

export interface GetStudentClassesQuery_Result {
  schoolClassId: string;
  schoolClassYear: number;
  schoolClassName: string;
  courseId: string;
  subjectCode: number;
  curriculumCode: number;
}
