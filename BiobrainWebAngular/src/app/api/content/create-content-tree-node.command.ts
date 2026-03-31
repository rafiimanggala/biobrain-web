import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateContentTreeNodeCommand extends Command<CreateContentTreeNodeCommand_Result[]> {
  constructor(
    public courseId: string,
    public parentId: string | null,
    public header: string,
    public order: number,
    public isAvailableInDemo: boolean
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/CreateContentTreeNode`;
  }

  deserializeResult(data: CreateContentTreeNodeCommand_Result_Object[]): CreateContentTreeNodeCommand_Result[] {
    return data.map(obj => new CreateContentTreeNodeCommand_Result(
      obj.entityId,
      obj.parentId,
      obj.header,
      obj.order,
      obj.depth,
      obj.isCanAttachContent,
      obj.isCanAddChildren,
      obj.isAvailableInDemo
    ));
  }
}

export class CreateContentTreeNodeCommand_Result {
  constructor(
    public readonly entityId: string,
    public readonly parentId: string,
    public header: string,
    public order: number,
    public readonly depth: number,
    public readonly isCanAttachContent: boolean,
    public readonly isCanAddChildren: boolean,
    public readonly isAvailableInDemo: boolean
  ) {
  }
}

export interface CreateContentTreeNodeCommand_Result_Object {
  entityId: string;
  parentId: string;
  header: string;
  order: number;
  depth: number;
  isCanAttachContent: boolean;
  isCanAddChildren: boolean;
  isAvailableInDemo: boolean;
}
