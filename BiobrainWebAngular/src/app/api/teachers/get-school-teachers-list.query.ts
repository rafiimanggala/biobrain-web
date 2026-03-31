import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetSchoolTeachersListQuery extends Query<GetSchoolTeachersListQuery_Result[]> {
  constructor(public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/GetSchoolTeachers`;
  }

  deserializeResult(data: GetSchoolTeachersListQuery_Result_Object[]): GetSchoolTeachersListQuery_Result[] {
    return data.map(obj => new GetSchoolTeachersListQuery_Result(
      obj.teacherId,
      obj.firstName,
      obj.lastName,
      obj.schoolId
    ));
  }
}

export class GetSchoolTeachersListQuery_Result {
  constructor(
    public readonly teacherId: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly schoolId: string
  ) {
  }

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
}

interface GetSchoolTeachersListQuery_Result_Object {
  teacherId: string;
  firstName: string;
  lastName: string;
  schoolId: string;
}
