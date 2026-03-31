import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateSchoolClassCommand extends Command<UpdateSchoolClassCommand_Result> {
  constructor(
    public readonly schoolClassId: string,
    public readonly year: number,
    public readonly name: string,
    public readonly teacherIds: string[],
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/UpdateSchoolClass`;
  }
}

export class UpdateSchoolClassCommand_Result {
}
