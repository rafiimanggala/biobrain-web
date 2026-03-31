import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class PreviewInsightsCommand extends Command<PreviewInsightsCommand_Result> {
  constructor(
    public readonly schoolClassId: string,
    public readonly courseId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.ai()}/preview-insights`;
  }
}

export interface PreviewInsightsCommand_Result {
  insights: string;
}
