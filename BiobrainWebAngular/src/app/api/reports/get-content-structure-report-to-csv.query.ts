import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetContentStructureReportToCsv extends Query<GetContentStructureReportToCsv_Result> {
  constructor(public readonly courseId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.report()}/GetContentStructureToCsv`;
  }
}

export interface GetContentStructureReportToCsv_Result {
  fileUrl: string,
}
