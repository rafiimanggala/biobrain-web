import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class DeleteStudentCommand extends Command<DeleteStudentCommand_Result> {
  constructor(public readonly studentId: string, public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/DeleteStudent`;
  }
}

export class DeleteStudentCommand_Result {
}
