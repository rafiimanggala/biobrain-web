import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetVouchersReport extends Query<GetVouchersReport_Result> {
  constructor(public readonly fromDateUtc: string, public readonly toDateUtc: string, public readonly timeZoneId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.report()}/VoucherUsedReportToCsv`;
  }
}

export interface GetVouchersReport_Result {
  fileUrl: string,
}
