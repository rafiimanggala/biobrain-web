
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class AddExistingStudentToSchoolCommand  extends Command<EmptyCommandResult>{
  constructor(
    public readonly email: string,
    public readonly schoolId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/AddExistingStudentToSchool`;
  }
}
