import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetExcludedMaterialsQuery extends Query<GetExcludedMaterialsQuery_Result> {
  constructor(public readonly schoolClassId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.learningMaterialAssignments()}/GetExcludedMaterials`;
  }
}

export interface GetExcludedMaterialsQuery_Result {
  readonly materialIds: string[];
}
