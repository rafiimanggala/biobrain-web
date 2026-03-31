import moment, { Moment } from 'moment';
import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetStudentsListQuery extends Query<GetStudentsListQuery_Result[]> {
  constructor(public readonly fromDateUtc: string, public readonly toDateUtc: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/GetStudents`;
  }

  deserializeResult(data: GetStudentsListQuery_Result_Object[]): GetStudentsListQuery_Result[] {
    return data.map(obj => new GetStudentsListQuery_Result(
      obj.studentId,
      obj.firstName,
      obj.lastName,
      obj.email,
      obj.registeredAt ? moment.utc(obj.registeredAt).local() : null,
      obj.subscribedAt ? moment.utc(obj.subscribedAt).local() : null,
      obj.licenseTypes
    ));
  }
}

export class GetStudentsListQuery_Result {
  constructor(
    public readonly studentId: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly email: string,
    public readonly registeredAt: Moment|null,
    public readonly subscribedAt: Moment|null,
    public readonly licenseTypes: string[]
  ) {
  }

  get fullName(): string {
    return `${this.firstName} ${this.lastName}`;
  }
}

export interface GetStudentsListQuery_Result_Object {
  studentId: string;
  firstName: string;
  lastName: string;
  email: string;
  registeredAt: string;
  subscribedAt: string;
  licenseTypes: string[];
}
