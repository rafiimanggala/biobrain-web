import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class PublishCourseCommand extends Command<PublishCourseCommand_Result> {
  constructor(
    public courseId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/IncrementVersion`;
  }

  deserializeResult(data: PublishCourseCommand_Result_Object): PublishCourseCommand_Result {
    return new PublishCourseCommand_Result(
      data.version
    );
  }
}

export class PublishCourseCommand_Result {
  constructor(
    public readonly version: string,
  ) {
  }
}

export interface PublishCourseCommand_Result_Object {
  version: string;
}
