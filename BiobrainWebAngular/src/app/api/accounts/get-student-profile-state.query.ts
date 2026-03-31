import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetStudentProfileStateQuery extends Query<StudentProfileState> {
  constructor(
    public userId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/GetStudentAccountState`;
  }
}

export interface StudentProfileState {
  readonly isFromArchivedSchool: string;
  readonly isCountryFieldsFilled: string;
  readonly isPaymentDetailsFilled: string;
}
