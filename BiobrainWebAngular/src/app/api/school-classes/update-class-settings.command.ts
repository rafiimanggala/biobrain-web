import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateClassSettingsCommand extends Command<UpdateClassSettingsCommand_Result> {
  constructor(
    public readonly schoolClassId: string,
    public readonly hintsDisabled: boolean,
    public readonly soundDisabled: boolean,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/UpdateClassSettings`;
  }
}

export interface UpdateClassSettingsCommand_Result {
  hintsDisabled: boolean;
  soundDisabled: boolean;
}
