import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetUserGuideContentQuery extends Query<GetUserGuideContentQuery_Result> {
  constructor(public readonly nodeId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userGuide()}/GetUserGuideContent`;
  }
}

export interface GetUserGuideContentQuery_Result {
  articleId: string;
  nodeId: string;
  htmlText: string;
  videoUrl: string;
}
