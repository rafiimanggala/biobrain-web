import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetAccessCodesBatchReportQuery extends Query<GetAccessCodesBatchReportQuery_Result> {
  constructor(
    public batchId: string,
    public timeZoneId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accessCodes()}/GetAccessCodesBatchReport`;
  }
}

export interface GetAccessCodesBatchReportQuery_Result {
  fileUrl: string
}