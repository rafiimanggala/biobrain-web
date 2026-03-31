import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SetQuizResultQuestionValueCommand extends Command<SetQuizResultQuestionValueCommand_Result> {
  constructor(
    public readonly quizResultId: string,
    public readonly questionId: string,
    public readonly localDate: Date,
    public readonly value: string,
    public readonly isCorrect: boolean) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/SetQuizResultQuestionValue`;
  }
}

export interface SetQuizResultQuestionValueCommand_Result {
}
