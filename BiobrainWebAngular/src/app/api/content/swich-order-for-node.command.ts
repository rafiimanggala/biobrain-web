import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SwichOrderForNodeCommand extends Command<SwichOrderForNodeCommand_Result> {
  constructor(
    public entity1Id: string,
    public entity2Id: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/SwitchOrderForNodes`;
  }

  deserializeResult(obj: SwichOrderForNodeCommand_Result_Object): SwichOrderForNodeCommand_Result {
    return new SwichOrderForNodeCommand_Result(
      obj.entity1Id,
      obj.order1,
      obj.entity2Id,
      obj.order2,
    );
  }
}

export class SwichOrderForNodeCommand_Result {
  constructor(
    public readonly entity1Id: string,
    public readonly order1: number,
    public readonly entity2Id: string,
    public readonly order2: number,
  ) {
  }
}

export interface SwichOrderForNodeCommand_Result_Object {
  entity1Id: string;
  order1: number;
  entity2Id: string;
  order2: number;
}
