import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class ImportContentCommand extends Command<ImportContentCommand_Result> {
  constructor(
    public courseId: string,
    public jsonContent: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/ImportContent`;
  }

  deserializeResult(data: ImportContentCommand_Result_Object): ImportContentCommand_Result {
    return new ImportContentCommand_Result(
      data.topicsCreated,
      data.subtopicsCreated,
      data.materialsCreated,
      data.questionsCreated,
      data.log
    );
  }
}

export class ImportContentCommand_Result {
  constructor(
    public topicsCreated: number,
    public subtopicsCreated: number,
    public materialsCreated: number,
    public questionsCreated: number,
    public log: string,
  ) {}
}

export interface ImportContentCommand_Result_Object {
  topicsCreated: number;
  subtopicsCreated: number;
  materialsCreated: number;
  questionsCreated: number;
  log: string;
}
