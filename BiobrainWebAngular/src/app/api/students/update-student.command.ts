import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateStudentCommand extends Command<UpdateStudentCommand_Result> {
  constructor(
    public readonly studentId: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly country: string,
    public readonly state: string| null | undefined,
    public readonly curriculumCode?: number| null | undefined,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/UpdateStudent`;
  }
}

export class UpdateStudentCommand_Result {
}
