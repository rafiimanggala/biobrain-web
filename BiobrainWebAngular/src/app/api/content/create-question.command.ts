import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export interface CreateQuestionAnswerInput {
  text: string;
  isCorrect: boolean;
  caseSensitive: boolean;
  score: number;
}

export class CreateQuestionCommand extends Command<CreateQuestionCommand_Result> {
  constructor(
    public courseId: string,
    public questionTypeCode: number,
    public header: string,
    public text: string,
    public hint: string,
    public feedBack: string,
    public answers: CreateQuestionAnswerInput[],
    public nodeId: string | null
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/CreateQuestion`;
  }

  deserializeResult(data: CreateQuestionCommand_Result_Object): CreateQuestionCommand_Result {
    return new CreateQuestionCommand_Result(data.questionId);
  }
}

export class CreateQuestionCommand_Result {
  constructor(public readonly questionId: string) {}
}

export interface CreateQuestionCommand_Result_Object {
  questionId: string;
}
