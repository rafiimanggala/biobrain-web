import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SignUpTeacherCommand extends Command<SignUpTeacherCommand_Result> {
  constructor(
    public readonly email: string,
    public readonly password: string,
    public readonly firstName: string,
    public readonly lastName: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/SignUpTeacher`;
  }
}


export interface SignUpTeacherCommand_Result {
  schoolId: string;
  teacherId: string;
}
