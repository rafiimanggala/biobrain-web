import { KeyValue } from '@angular/common';
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AttachQuestionsToNodeCommand extends Command<AttachQuestionsToNodeCommand_Result>{
  constructor(
    public nodeId: string,
    public questionIds: Array<KeyValue<number, string>>,
    public isReplaceMode: boolean
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/AttachQuestionsToNode`;
  }
}

export class AttachQuestionsToNodeCommand_Result {
}
