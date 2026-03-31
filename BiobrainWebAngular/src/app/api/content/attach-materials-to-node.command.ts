import { KeyValue } from '@angular/common';
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AttachMaterialsToNodeCommand extends Command<AttachMaterialsToNodeCommand_Result> {
  constructor(
    public nodeId: string,
    public materialIds: Array<KeyValue<number, string>>,
    public isReplaceMode: boolean
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/AttachMaterialsToNode`;
  }
}

export class AttachMaterialsToNodeCommand_Result {
}
