import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GetUsageReport extends Command<GetUsageReport_Result> {
  constructor(public readonly schoolId: string, public readonly fromDateUtc: string, public readonly toDateUtc: string, public readonly timeZoneId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.report()}/GetUsageReport`;
  }
}

export interface GetUsageReport_Result {
  fileUrl: string,
}
