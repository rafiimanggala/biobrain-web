import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateTeacherCommand extends Command<CreateTeacherCommand_Result> {
  constructor(
    public readonly schoolId: string,
    public readonly email: string,
    public readonly firstName: string,
    public readonly lastName: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/CreateTeacher`;
  }

  deserializeResult(data: CreateTeacherCommand_Result_Object): CreateTeacherCommand_Result {
    return new CreateTeacherCommand_Result(
      data.teacherId,
      data.schoolId,
      data.firstName,
      data.lastName
    );
  }
}

export class CreateTeacherCommand_Result {
  constructor(
    public readonly teacherId: string,
    public readonly schoolId: string,
    public readonly firstName: string,
    public readonly lastName: string
  ) {
  }
}

interface CreateTeacherCommand_Result_Object {
  teacherId: string;
  schoolId: string;
  firstName: string;
  lastName: string;
}
