import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateSchoolClassCommand extends Command<CreateSchoolClassCommand_Result> {
  constructor(
    public readonly schoolId: string,
    public readonly courseId: string,
    public readonly year: number,
    public readonly name: string,
    public readonly teacherIds: string[]
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/CreateSchoolClass`;
  }

  deserializeResult(obj: CreateSchoolClassCommand_Result_Object): CreateSchoolClassCommand_Result {
    return new CreateSchoolClassCommand_Result(obj.schoolClassId);
  }
}

export class CreateSchoolClassCommand_Result {
  constructor(public readonly schoolClassId: string) {
  }
}

export interface CreateSchoolClassCommand_Result_Object {
  schoolClassId: string;
}
