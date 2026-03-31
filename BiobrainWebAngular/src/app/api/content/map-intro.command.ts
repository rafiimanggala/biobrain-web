import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class MapCourseCommand extends Command<EmptyCommandResult> {
  constructor(
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/MapCourses`;
  }
}
