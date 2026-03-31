import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetBookmarksQuery extends Query<GetBookmarksQuery_Result[]> {
  constructor(
    public readonly userId: string,
    public readonly courseId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.bookmarks()}/GetBookmarksForUser`;
  }
}

export interface GetBookmarksQuery_Result {
  readonly bookmarkId: string;
  readonly materialId: string;
  readonly nodeId: string;
  readonly levelId: string;
  readonly path: string[];
  readonly header: string;
}
