import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SignUpStudentCommand extends Command<SignUpStudentCommand_Result> {
  constructor(
    public readonly email: string,
    public readonly password: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly classCode: string,
    public readonly country: string,
    public readonly state: string,
    public readonly curriculumCode?: number
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/SignUpStudent`;
  }
}


export interface SignUpStudentCommand_Result {
  schoolId: string;
  schoolClassId: string;
  studentId: string;
}
