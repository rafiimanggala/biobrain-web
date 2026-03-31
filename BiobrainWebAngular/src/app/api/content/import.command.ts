import { ApiPath } from '../api-path.service';
import { FileCommand } from '../common/file-command';

export class ImportCommand extends FileCommand<ImportCommand_Result[]> {
  constructor(
    file: File,
  ) {
    super(file);
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/Import`;
  }

  deserializeResult(data: ImportCommand_Result_Object[]): ImportCommand_Result[] {
    return data.map(obj => new ImportCommand_Result(
      obj.courseId,
      obj.rowsAdded,
      obj.rowsUpdated,
      obj.rowsDeleted,
      obj.log
    ));
  }
}

export class ImportCommand_Result {
  constructor(
    public courseId: string,
    public rowsAdded: number,
    public rowsUpdated: number,
    public rowsDeleted: number,
    public log: string,
  ) {
  }
}

export interface ImportCommand_Result_Object {
  courseId: string;
  rowsAdded: number;
  rowsUpdated: number;
  rowsDeleted: number;
  log: string;
}
