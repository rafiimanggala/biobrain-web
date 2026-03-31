import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateStudentCommand extends Command<CreateStudentCommand_Result> {
  constructor(
    public readonly schoolId: string,
    public readonly email: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly country: string,
    public readonly state: string,
    public readonly curriculumCode: number,
    public readonly year: number
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.students()}/CreateStudent`;
  }

  deserializeResult(data: CreateStudentCommand_Result_Object): CreateStudentCommand_Result {
    return new CreateStudentCommand_Result(data.studentId);
  }
}

export class CreateStudentCommand_Result {
  constructor(public readonly studentId: string) {
  }
}

export interface CreateStudentCommand_Result_Object {
  studentId: string;
}
