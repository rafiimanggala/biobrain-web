import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AddTeacherByEmailCommand extends Command<AddTeacherByEmailCommand_Result> {
  constructor(
    public readonly schoolId: string,
    public readonly email: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/AddTeacherByEmail`;
  }
}

export class AddTeacherByEmailCommand_Result {
  constructor(
  ) {
  }
}
