import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetTeacherByIdQuery extends Query<GetTeacherByIdQuery_Result> {
  constructor(public readonly teacherId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/GetById`;
  }

  deserializeResult(data: GetTeacherByIdQuery_Result_Object): GetTeacherByIdQuery_Result {
    return new GetTeacherByIdQuery_Result(
      data.teacherId,
      data.email,
      data.firstName,
      data.lastName
    );
  }
}

interface GetTeacherByIdQuery_Result_Object {
  teacherId: string;
  email: string;
  firstName: string;
  lastName: string;
}

export class GetTeacherByIdQuery_Result {
  constructor(
    public readonly teacherId: string,
    public readonly email: string,
    public readonly firstName: string,
    public readonly lastName: string,
  ) {
  }

  public get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
}
