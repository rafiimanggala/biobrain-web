import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';
import { TreeMode } from '../enums/tree-mode.enum';

export class GetContentTreeListQuery extends Query<GetContentTreeListQuery_Result[]> {
  constructor(
    public courseId: string,
    public mode: TreeMode
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetCourseContentTree`;
  }

  deserializeResult(data: GetContentTreeListQuery_Result_Object[]): GetContentTreeListQuery_Result[] {
    return data.map(model => GetContentTreeListQuery_Result.deserialize(model));
  }
}

export class GetContentTreeListQuery_Result {
  isSelected: boolean = false;

  constructor(
    public readonly entityId: string,
    public readonly parentId: string,
    public header: string,
    public order: number,
    public readonly depth: number,
    public readonly children: GetContentTreeListQuery_Result[],
    public readonly isCanAttachContent: boolean,
    public readonly isCanAddChildren: boolean,
    public readonly isCanCopyIn: boolean,
    public readonly isMaterialsFolder: boolean,
    public readonly isQuestionsFolder: boolean,
    public isAutoMapped: boolean,
    public isExcluded: boolean,
    public isAvailableInDemo: boolean
  ) {
  }

  static deserialize(
    obj: GetContentTreeListQuery_Result_Object
  ): GetContentTreeListQuery_Result {
    return new GetContentTreeListQuery_Result(
      obj.entityId,
      obj.parentId,
      obj.header,
      obj.order,
      obj.depth,
      obj.children.map(x => GetContentTreeListQuery_Result.deserialize(x)),
      obj.isCanAttachContent,
      obj.isCanAddChildren,
      obj.isCanCopyIn,
      obj.isMaterialsFolder,
      obj.isQuestionsFolder,
      obj.isAutoMapped,
      obj.isExcluded,
      obj.isAvailableInDemo
    );
  }

  update(
    header: string,
    order: number,
    isAvailableInDemo: boolean
  ) {
    this.header = header;
    this.order = order;
    this.isAvailableInDemo = isAvailableInDemo;
  }
}

export interface GetContentTreeListQuery_Result_Object {
  entityId: string;
  parentId: string;
  header: string;
  order: number;
  depth: number;
  children: GetContentTreeListQuery_Result_Object[];
  isCanAttachContent: boolean;
  isCanAddChildren: boolean;
  isCanCopyIn: boolean;
  isMaterialsFolder: boolean;
  isQuestionsFolder: boolean;
  isAutoMapped: boolean;
  isExcluded: boolean;
  isAvailableInDemo: boolean;
}
