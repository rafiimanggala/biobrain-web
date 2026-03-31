import { ApiPath } from '../api-path.service';
import { FileCommand } from '../common/file-command';
import { ImageCommand } from '../common/image-command';

export class UploadUserGuideImageCommand extends FileCommand<UploadUserGuideImageCommand_Result> {
  constructor(
    file: File,
  ) {
    super(file);
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.userGuide()}/UploadUserGuideImage`;
  }
}

export interface UploadUserGuideImageCommand_Result {
  fileLink: string;
}
