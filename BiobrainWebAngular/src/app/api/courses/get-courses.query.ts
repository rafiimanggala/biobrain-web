import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetCoursesQuery extends Query<GetCoursesQuery_Result[]>{
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.courses()}/GetCourses`;
  }
}

export interface GetCoursesQuery_Result {
  courseId: string;
  name:string;
  subjectCode: number;
  curriculumCode: number;
  year: number;
  subHeader: string | null;
}
