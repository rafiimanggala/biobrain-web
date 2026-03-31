import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetBaseMaterialsListQuery extends Query<GetBaseMaterialsListQuery_Result[]> {
  constructor(
    public baseCourseId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetBaseMaterials`;
  }

  deserializeResult(data: GetBaseMaterialsListQuery_Result_Object[]): GetBaseMaterialsListQuery_Result[] {
    return data.map(obj => new GetBaseMaterialsListQuery_Result(
      obj.entityId,
      obj.path
    ));
  }
}

export class GetBaseMaterialsListQuery_Result {
  constructor(
    public readonly entityId: string,
    public readonly path: string[]
  ) {
  }
}

export interface GetBaseMaterialsListQuery_Result_Object {
  entityId: string;
  path: string[];
}
