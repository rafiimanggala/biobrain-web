import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class DeleteBookmarkForUserCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly userId: string,
    public readonly bookmarkId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.bookmarks()}/DeleteBookmarkForUser`;
  }
}

