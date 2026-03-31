import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class DeleteSchoolCommand extends Command<DeleteSchoolCommand_Result> {
  constructor(public readonly schoolId: string) {
    super();
  }
  
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/DeleteSchool`;
  }
}

export class DeleteSchoolCommand_Result {
}
