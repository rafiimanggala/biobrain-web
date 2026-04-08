import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export interface UpdateQuizSettingsCommand_Result {
  quizId: string;
}

export class UpdateQuizSettingsCommand extends Command<UpdateQuizSettingsCommand_Result> {
  constructor(
    public quizId: string,
    public name: string | null,
    public questionCount: number | null
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/UpdateQuizSettings`;
  }

  deserializeResult(data: any): UpdateQuizSettingsCommand_Result {
    return { quizId: data.quizId };
  }
}
