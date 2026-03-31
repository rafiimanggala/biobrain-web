import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class DeleteTeacherCommand extends Command<DeleteTeacherCommand_Result> {
  constructor(public readonly teacherId: string, public readonly schoolId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/DeleteTeacher`;
  }
}

export class DeleteTeacherCommand_Result {
}
