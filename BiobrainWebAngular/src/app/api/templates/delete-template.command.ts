import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { EmptyCommandResult } from '../empty-command-result';

export class DeleteTemplateCommand extends Command<EmptyCommandResult> {
  constructor(public readonly templateId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.templates()}/DeleteTemplate`;
  }
}
