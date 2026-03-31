
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class InviteStudentByEmailCommand extends Command<InviteStudentByEmailCommand_Result> {
  constructor(
    public readonly email: string,
    public readonly schoolClassId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/InviteStudentByEmail`;
  }
}

export interface InviteStudentByEmailCommand_Result {
  isStudentAddedToClass: boolean;
}
