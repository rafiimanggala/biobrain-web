import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetSchoolStudentsListQuery extends Query<GetSchoolStudentsListQuery_Result[]> {
  constructor(public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/GetSchoolStudents`;
  }

  deserializeResult(data: GetSchoolStudentsListQuery_Result_Object[]): GetSchoolStudentsListQuery_Result[] {
    return data.map(obj => new GetSchoolStudentsListQuery_Result(
      obj.studentId,
      obj.firstName,
      obj.lastName,
      obj.email,
      obj.schoolId,
      obj.schoolClassIds,
      obj.schoolClassNames
    ));
  }
}

export class GetSchoolStudentsListQuery_Result {
  constructor(
    public readonly studentId: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly email: string,
    public readonly schoolId: string | null | undefined,
    public readonly schoolClassIds: string[],
    public readonly schoolClassNames: string[]
  ) {
  }

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
}

export interface GetSchoolStudentsListQuery_Result_Object {
  studentId: string;
  firstName: string;
  lastName: string;
  email: string;
  schoolId: string | null | undefined;
  schoolClassIds: string[];
  schoolClassNames: string[];
}
