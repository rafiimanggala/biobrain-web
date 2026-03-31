
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class AddTeacherToSchoolClassCommand  extends Command<EmptyCommandResult>{
  constructor(
    public readonly teacherId: string,
    public readonly schoolClassId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/AddTeacherToSchoolClass`;
  }
}
