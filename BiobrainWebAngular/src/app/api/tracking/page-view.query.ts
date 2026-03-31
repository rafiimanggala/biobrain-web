import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';
import { EmptyCommandResult } from '../empty-command-result';

export class PageViewQuery extends Query<EmptyCommandResult> {
  constructor(
    public readonly pagePath: string, public readonly courseId: string|null, public readonly schoolId: string|null
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userTracking()}/PageView`;
  }
}
