
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class RenameClassCommand  extends Command<EmptyCommandResult>{
  constructor(
    public readonly name: string,
    public readonly schoolClassId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/RenameSchoolClass`;
  }
}
