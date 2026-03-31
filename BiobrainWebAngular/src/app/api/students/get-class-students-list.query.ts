import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetClassStudentsListQuery extends Query<GetClassStudentsListQuery_Result[]> {
  constructor(public readonly schoolId: string, public readonly schoolClassId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/GetSchoolClassStudents`;
  }

  deserializeResult(data: GetClassStudentsListQuery_Result_Object[]): GetClassStudentsListQuery_Result[] {
    return data.map(obj => new GetClassStudentsListQuery_Result(
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

export class GetClassStudentsListQuery_Result {
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

export interface GetClassStudentsListQuery_Result_Object {
  studentId: string;
  firstName: string;
  lastName: string;
  email: string;
  schoolId: string | null | undefined;
  schoolClassIds: string[];
  schoolClassNames: string[];
}
