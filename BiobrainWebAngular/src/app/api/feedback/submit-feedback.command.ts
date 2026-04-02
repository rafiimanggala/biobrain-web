import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SubmitFeedbackCommand extends Command<SubmitFeedbackCommand_Result> {
  constructor(
    public readonly rating: number,
    public readonly feedbackText: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.feedback()}/SubmitFeedback`;
  }
}

export interface SubmitFeedbackCommand_Result {
}
