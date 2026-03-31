import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class DeleteSchoolClassCommand extends Command<DeleteSchoolClassCommand_Result> {
  constructor(public readonly schoolClassId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/DeleteSchoolClass`;
  }
}

export class DeleteSchoolClassCommand_Result {
}
