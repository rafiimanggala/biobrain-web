import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class UpdateStudentClassesCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly studentId: string,
    public readonly schoolId: string,
    public readonly schoolClassIds: string[]) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/UpdateStudentClasses`;
  }
}
