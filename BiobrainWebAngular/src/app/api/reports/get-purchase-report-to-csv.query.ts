import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetPurchaseReportToCsv extends Query<GetPurchaseReportToCsv_Result> {
  constructor(public readonly fromDateUtc: string, public readonly toDateUtc: string, public readonly timeZoneId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.report()}/GetPurchaseReportToCsv`;
  }
}

export interface GetPurchaseReportToCsv_Result {
  fileUrl: string,
}
