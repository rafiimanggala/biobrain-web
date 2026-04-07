import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AddExcludedMaterialCommand extends Command<AddExcludedMaterialCommand_Result> {

  constructor(
    public schoolClassId: string,
    public materialId: string,
    public isExcluded: boolean
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.learningMaterialAssignments()}/AddExcludedMaterial`;
  }
}


export interface AddExcludedMaterialCommand_Result {
}
