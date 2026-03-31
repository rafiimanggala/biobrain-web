import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CopyNodeCommand extends Command<CopyNodeCommand_Result[]> {
  constructor(
    public parentId: string,
    public nodeIds: string[],
    // public isReplaceMode: boolean
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/CopyNode`;
  }
}

export class CopyNodeCommand_Result {
  constructor(
  public readonly entityId: string,
  public readonly parentId: string,
  public header: string,
  public order: number,
  public readonly depth: number,
  public readonly isCanAttachContent: boolean,
  public readonly isCanAddChildren: boolean
  ){}
}
