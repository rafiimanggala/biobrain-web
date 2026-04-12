import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AskBiobrainCommand extends Command<AskBiobrainCommand_Result> {
  constructor(
    public readonly courseId: string,
    public readonly contentTreeNodeId: string,
    public readonly question: string,
    public readonly conversationHistory: AskBiobrainMessage[]
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.ai()}/Ask`;
  }
}

export interface AskBiobrainMessage {
  role: 'user' | 'assistant';
  content: string;
}

export interface AskBiobrainCommand_Result {
  answer: string;
}
