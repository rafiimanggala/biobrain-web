import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateBookmarkForUserCommand extends Command<CreateBookmarkForUserCommand_Result> {
  constructor(
    public readonly userId: string,
    public readonly courseId: string,
    public readonly materialId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.bookmarks()}/CreateBookmarkForUser`;
  }
}

export interface CreateBookmarkForUserCommand_Result {
  readonly bookmarkId: string;
  readonly materialId: string;
  readonly courseId: string,
}

