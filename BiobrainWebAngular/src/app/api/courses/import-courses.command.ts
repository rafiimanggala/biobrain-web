import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class ImportCoursesCommand extends Command<ImportCoursesCommand_Result> {
  constructor(
    public courses: CourseImportItem[],
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.courses()}/ImportCourses`;
  }

  deserializeResult(data: ImportCoursesCommand_Result_Object): ImportCoursesCommand_Result {
    return new ImportCoursesCommand_Result(
      data.successCount,
      data.errorCount,
      data.errors
    );
  }
}

export class ImportCoursesCommand_Result {
  constructor(
    public successCount: number,
    public errorCount: number,
    public errors: string[],
  ) {}
}

export interface ImportCoursesCommand_Result_Object {
  successCount: number;
  errorCount: number;
  errors: string[];
}

export interface CourseImportItem {
  subjectCode: number;
  curriculumCode: number;
  year: number;
  subHeader: string;
  postfix: string;
  isForSell: boolean;
  isBase: boolean;
  courseGroup: number;
}
