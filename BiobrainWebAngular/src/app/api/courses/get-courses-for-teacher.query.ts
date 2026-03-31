import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetCoursesForTeacherQuery extends Query<GetCoursesForStudentQuery_Result[]> {
  constructor(public readonly teacherId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.courses()}/GetCoursesForTeacher`;
  }
}

export interface GetCoursesForStudentQuery_Result {

  teacherId: string;
  schoolId: string;
  schoolName: string;
  courses: GetCoursesForStudentQuery_Result_Course[];
}

export interface GetCoursesForStudentQuery_Result_Course {
  courseId: string;
  classId: string;
  className: string;
  classYear: number;
}
