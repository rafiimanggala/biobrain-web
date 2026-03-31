import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class AddNewQuestionsCommand extends Command<AddNewQuestionsCommand_Result[]> {
  constructor(
    public courseId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/AddAllNewQuestions`;
  }
}

export interface AddNewQuestionsCommand_Result {
  nodeName: string;
  numberOfQuestions: number;
}
