import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SignUpStandaloneStudentCommand extends Command<SignUpStandaloneStudentCommand_Result> {
  constructor(
    public readonly email: string,
    public readonly password: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly country: string,
    public readonly state: string,
    public readonly year: number,
    public readonly curriculumCode?: number
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/SignUpStandaloneStudent`;
  }
}


export interface SignUpStandaloneStudentCommand_Result {
}
