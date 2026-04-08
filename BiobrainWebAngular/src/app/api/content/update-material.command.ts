import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateMaterialCommand extends Command<void> {
  constructor(
    public materialId: string,
    public header: string,
    public text: string,
    public videoLink: string | null
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/UpdateMaterial`;
  }

  deserializeResult(_: any): void {
    return;
  }
}
