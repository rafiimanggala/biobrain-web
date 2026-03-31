import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetLatestWhatsNewQuery extends Query<GetLatestWhatsNewQuery_Result> {
  constructor() {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.whatsNew()}/GetLatest`;
  }
}

export interface GetLatestWhatsNewQuery_Result {
  whatsNewId: string | null;
  title: string;
  content: string;
  version: string;
  publishedAt: string | null;
}
