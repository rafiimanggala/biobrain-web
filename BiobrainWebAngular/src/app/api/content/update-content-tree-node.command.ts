import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateContentTreeNodeCommand extends Command<UpdateContentTreeNodeCommand_Result> {
  constructor(
    public entityId: string,
    public header: string,
    public isAvailableInDemo: boolean
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/UpdateContentTreeNode`;
  }

  deserializeResult(obj: UpdateContentTreeNodeCommand_Result_Object): UpdateContentTreeNodeCommand_Result {
    return new UpdateContentTreeNodeCommand_Result(
      obj.entityId,
      obj.parentId,
      obj.header,
      obj.order,
      obj.depth,
      obj.isCanAttachContent,
      obj.isCanAddChildren,
      obj.isAvailableInDemo
    );
  }
}

export class UpdateContentTreeNodeCommand_Result {
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

export interface UpdateContentTreeNodeCommand_Result_Object {
  entityId: string;
  parentId: string;
  header: string;
  order: number;
  depth: number;
  isCanAttachContent: boolean;
  isCanAddChildren: boolean;
  isAvailableInDemo: boolean;
}
