import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetStudentByIdQuery extends Query<GetStudentByIdQuery_Result> {
  constructor(public readonly studentId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/GetById`;
  }

  deserializeResult(obj: GetStudentByIdQuery_Result_Object): GetStudentByIdQuery_Result {
    return new GetStudentByIdQuery_Result(
      obj.studentId,
      obj.email,
      obj.firstName,
      obj.lastName,
      obj.country,
      obj.state,
      obj.curriculumCode,
      obj.year,
      obj.schoolClassIds
    );
  }
}

export class GetStudentByIdQuery_Result {
  constructor(
    public readonly studentId: string,
    public readonly email: string,
    public readonly firstName: string,
    public readonly lastName: string,
    
    public readonly country: string,
    public readonly state: string,
    public readonly curriculumCode: number,
    public readonly year: number,

    public readonly schoolClassIds: string[]
  ) {
  }

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
}

export interface GetStudentByIdQuery_Result_Object {
  studentId: string;
  email: string;
  firstName: string;
  lastName: string;
  schoolClassIds: string[];
  country: string;
  state: string;
  curriculumCode: number;
  year: number;

}
