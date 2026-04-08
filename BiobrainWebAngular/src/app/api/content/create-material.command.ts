import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class CreateMaterialCommand extends Command<CreateMaterialCommand_Result> {
  constructor(
    public courseId: string,
    public header: string,
    public text: string,
    public videoLink: string | null,
    public nodeId: string | null
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/CreateMaterial`;
  }

  deserializeResult(data: CreateMaterialCommand_Result_Object): CreateMaterialCommand_Result {
    return new CreateMaterialCommand_Result(data.materialId);
  }
}

export class CreateMaterialCommand_Result {
  constructor(public readonly materialId: string) {}
}

export interface CreateMaterialCommand_Result_Object {
  materialId: string;
}
