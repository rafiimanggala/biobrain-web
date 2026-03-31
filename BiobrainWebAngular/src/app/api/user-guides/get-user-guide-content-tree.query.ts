import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetUserGuideContentTreeQuery extends Query<GetUserGuideContentTreeQuery_Result[]> {
  constructor() {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userGuide()}/GetUserGuideContentTree`;
  }
}

export interface GetUserGuideContentTreeQuery_Result {
  nodeId: string;
  parentId: string;
  name: string;
  order: number;
  isAvailableForStudent: boolean;
  children: GetUserGuideContentTreeQuery_Result[];
}
