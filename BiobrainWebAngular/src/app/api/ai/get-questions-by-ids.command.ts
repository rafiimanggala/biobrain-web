import { Question } from '../content/content-data-models';
import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GetQuestionsByIdsCommand extends Command<GetQuestionsByIdsCommand_Result> {
  constructor(
    public readonly questionIds: string[],
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.ai()}/GetQuestionsByIds`;
  }
}

export interface GetQuestionsByIdsCommand_Result {
  readonly questions: Question[];
}
