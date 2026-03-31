import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateTeacherDetailsCommand extends Command<UpdateTeacherDetailsCommand_Result> {
  constructor(
    public readonly teacherId: string,
    public readonly firstName: string,
    public readonly lastName: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.teachers()}/UpdateTeacherDetails`;
  }

  deserializeResult(data: UpdateTeacherDetailsCommand_Result_Object): UpdateTeacherDetailsCommand_Result {
    return new UpdateTeacherDetailsCommand_Result(
      data.teacherId,
      data.firstName,
      data.lastName
    );
  }
}

export class UpdateTeacherDetailsCommand_Result {
  constructor(
    public readonly teacherId: string,
    public readonly firstName: string,
    public readonly lastName: string
  ) {
  }
}

interface UpdateTeacherDetailsCommand_Result_Object {
  teacherId: string;
  firstName: string;
  lastName: string;
}
