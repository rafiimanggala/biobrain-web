import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetContentTreeMetaListQuery extends Query<GetContentTreeMetaListQuery_Result[]> {
  constructor(public courseId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetCourseContentTreeMeta`;
  }

  deserializeResult(data: GetContentTreeMetaListQuery_Result_Object[]): GetContentTreeMetaListQuery_Result[] {
    return data.map(model => GetContentTreeMetaListQuery_Result.deserialize(model));
  }
}

export class GetContentTreeMetaListQuery_Result {
  constructor(
    public readonly contentTreeMetaId: string,
    public readonly name: string,
    public readonly depth: number,
    public readonly isCanAddChildren: boolean,
    public readonly isCanAttachContent: boolean,
    public readonly isCanCopyIn: boolean
  ) {
  }

  static deserialize(
    obj: GetContentTreeMetaListQuery_Result_Object
  ): GetContentTreeMetaListQuery_Result {
    return new GetContentTreeMetaListQuery_Result(
      obj.contentTreeMetaId,
      obj.name,
      obj.depth,
      obj.isCanAddChildren,
      obj.isCanAttachContent,
      obj.isCanCopyIn
    );
  }
}

export interface GetContentTreeMetaListQuery_Result_Object {
  contentTreeMetaId: string;
  name: string;
  depth: number;
  isCanAddChildren: boolean;
  isCanAttachContent: boolean;
  isCanCopyIn: boolean;
}
