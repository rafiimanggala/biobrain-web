import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { Query } from '../common/query';

export class GetCoursesForStudentQuery extends Command<GetCoursesForStudentQuery_Result[]> {
  constructor(public readonly studentId: string, public readonly localDateTime: Date) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.courses()}/GetCoursesForStudent`;
  }
}

export interface GetCoursesForStudentQuery_Result {

  studentId: string;
  schoolId: string;
  schoolName: string;
  courses: GetCoursesForStudentQuery_Result_Course[];
}

export interface GetCoursesForStudentQuery_Result_Course {
  courseId: string;
  courseName: string;
  classId: string,
  className: string,
  classYear: number,
  streak: number
}
