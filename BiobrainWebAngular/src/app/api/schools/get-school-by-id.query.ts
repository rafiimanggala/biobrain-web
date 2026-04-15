import moment from 'moment';
import { Moment } from 'moment';
import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';
import { SchoolStatus } from '../enums/school-status.enum';

export class GetSchoolByIdQuery extends Query<GetSchoolByIdQuery_Result> {
  constructor(public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/GetById`;
  }

  deserializeResult(obj: GetSchoolByIdQuery_Result_Object): GetSchoolByIdQuery_Result {
    return new GetSchoolByIdQuery_Result(
      obj.schoolId,
      obj.name,
      obj.teachersLicensesNumber,
      obj.studentLicensesNumber,
      obj.useAccessCodes,
      obj.admins,
      obj.courses ?? [],
      obj.status,
      moment.utc(obj.startDateUtc),
      obj.endDateUtc ? moment.utc(obj.endDateUtc) : undefined,
      obj.aiDisabled ?? false
    );
  }
}

export class GetSchoolByIdQuery_Result {
  constructor(
    public readonly schoolId: string,
    public readonly name: string,
    public readonly teachersLicensesNumber: number,
    public readonly studentLicensesNumber: number,
    public readonly useAccessCodes: boolean,
    public readonly admins: string[] | undefined,
    public readonly courses: string[],
    public readonly status: SchoolStatus | undefined,
    public readonly startDate: Moment,
    public readonly endDate?: Moment | undefined,
    public readonly aiDisabled: boolean = false
  ) {
  }
}

export interface GetSchoolByIdQuery_Result_Object {
  schoolId: string;
  name: string;
  teachersLicensesNumber: number;
  studentLicensesNumber: number;
  useAccessCodes: boolean;
  status: SchoolStatus;
  admins: string[] | undefined;
  courses: string[] | undefined;
  startDateUtc: string;
  endDateUtc: string;
  aiDisabled: boolean;
}
