import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetSchoolLicenseInfoQuery extends Query<GetSchoolLicenseInfoQuery_Result> {
  constructor(public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/GetSchoolLicenseInfo`;
  }
}

export interface GetSchoolLicenseInfoQuery_Result {
  schoolId: string;
  teachersLicensesNumber: number;
  actualTeachersCount: number;
  studentLicensesNumber: number;
  actualStudentsCount: number;
}
