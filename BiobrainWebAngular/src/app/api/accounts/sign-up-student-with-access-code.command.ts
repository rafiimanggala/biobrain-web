import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SignUpStudentWithAccessCodeCommand extends Command<SignUpStudentWithAccessCodeCommand_Result> {
  constructor(
    public readonly email: string,
    public readonly password: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly accessCode: string,
    public readonly country: string,
    public readonly state: string,
    public readonly curriculumCode?: number
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/SignUpStudentWithAccessCode`;
  }
}


export interface SignUpStudentWithAccessCodeCommand_Result {
  studentId: string;
}
