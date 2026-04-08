import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { CreateQuestionAnswerInput } from './create-question.command';

export class UpdateQuestionCommand extends Command<void> {
  constructor(
    public questionId: string,
    public questionTypeCode: number,
    public header: string,
    public text: string,
    public hint: string,
    public feedBack: string,
    public answers: CreateQuestionAnswerInput[]
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/UpdateQuestion`;
  }

  deserializeResult(_: any): void {
    return;
  }
}
