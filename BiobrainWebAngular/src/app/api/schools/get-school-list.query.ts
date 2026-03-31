import moment from 'moment';
import { Moment } from 'moment';
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { SchoolStatus } from '../enums/school-status.enum';

export class GetSchoolListQuery extends Command<SchoolListModel_Result[]> {
  constructor(public readonly ids: string[] = []){
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/Get`;
  }

  deserializeResult(data: SchoolListModel_Result_Object[]): SchoolListModel_Result[] {
    return data.map(obj => new SchoolListModel_Result(
      obj.schoolId,
      obj.name,
      obj.teachersLicensesNumber,
      obj.studentLicensesNumber,
      obj.teachersCount,
      obj.studentsCount,
      obj.status,
      moment.utc(obj.startDateUtc),
      obj.endDateUtc ? moment.utc(obj.endDateUtc) : undefined
    ));
  }
}

export class SchoolListModel_Result {
  constructor(
    public readonly schoolId: string,
    public readonly name: string,
    public readonly teachersLicensesNumber: number,
    public readonly studentLicensesNumber: number,
    public readonly teachersCount: number,
    public readonly studentsCount: number,
    public readonly status: SchoolStatus,
    public readonly startDate: Moment,
    public readonly endDate?: Moment
  ) {
  }
}

export interface SchoolListModel_Result_Object {
  schoolId: string;
  name: string;
  teachersLicensesNumber: number;
  studentLicensesNumber: number;
  teachersCount: number;
  studentsCount: number;
  status: SchoolStatus;
  startDateUtc: string;
  endDateUtc: string;
}
