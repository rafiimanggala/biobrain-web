import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class UpdateTeacherClassesCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly teacherId: string,
    public readonly schoolClassIds: string[]) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/UpdateTeacherClasses`;
  }
}
