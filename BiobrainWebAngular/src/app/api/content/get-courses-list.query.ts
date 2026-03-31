import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetCoursesListQuery extends Query<GetCoursesListQuery_Result[]> {
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetCourses`;
  }

  deserializeResult(data: GetCoursesListQuery_Result_Object[]): GetCoursesListQuery_Result[] {
    return data.map(model => GetCoursesListQuery_Result.deserialize(model));
  }
}

export class GetCoursesListQuery_Result {
  constructor(
    public readonly courseId: string,
    public readonly name: string,
    public readonly isBase: boolean,
    public readonly isGeneric: boolean,
    public readonly subjectCode: number
  ) {
  }

  static deserialize(
    obj: GetCoursesListQuery_Result_Object
  ): GetCoursesListQuery_Result {
    return new GetCoursesListQuery_Result(
      obj.courseId,
      obj.name,
      obj.isBase,
      obj.isGeneric,
      obj.subjectCode
    );
  }
}

export interface GetCoursesListQuery_Result_Object {
  courseId: string;
  name: string;
  isBase: boolean;
  isGeneric: boolean;
  subjectCode: number;
}
