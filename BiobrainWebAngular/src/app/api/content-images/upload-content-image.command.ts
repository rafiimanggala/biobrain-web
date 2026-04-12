import { ApiPath } from '../api-path.service';
import { FileCommand } from '../common/file-command';

export class UploadContentImageCommand extends FileCommand<UploadContentImageResult> {
  constructor(
    file: File,
    public readonly code: string,
    public readonly description: string = '',
  ) {
    super(file);
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.contentImages()}/Upload`;
  }
}

export interface UploadContentImageResult {
  readonly imageId: string;
  readonly fileLink: string;
  readonly code: string;
}
