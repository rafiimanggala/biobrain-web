import { GuidCollectionUpdateModel } from 'src/app/core/api/models/guid-collection-update.model';
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateSchoolAdminsCommand extends Command<UpdateSchoolAdminsCommand_Result> {
  constructor(
    public readonly schoolId: string,
    public readonly teachers: GuidCollectionUpdateModel
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/UpdateSchoolAdmins`;
  }
}

export class UpdateSchoolAdminsCommand_Result {
}
