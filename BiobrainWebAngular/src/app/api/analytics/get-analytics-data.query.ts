import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetAnalyticsDataQuery extends Query<AnalyticsData> {
  constructor(
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.base}/googleanalytics`;
  }
}

export interface AnalyticsData {
  readonly trackingNumber: string;
}
