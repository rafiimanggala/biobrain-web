import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class JoinStudentToClassCommand extends Command<SignUpStudentCommand_Result> {
  constructor(
    public readonly studentId: string,
    public readonly classCode: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/JoinStudentToClass`;
  }
}


export interface SignUpStudentCommand_Result {
  className: string;
}
