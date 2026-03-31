import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class SaveTemplateCommand extends Command<EmptyCommandResult> {
  constructor(
    public readonly templateId: string | null,
    public readonly template: string,
    public readonly templateType: number,
    public readonly courses: string[]
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.templates()}/SaveTemplate`;
  }
}
