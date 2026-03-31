import { TemplateTypes } from 'src/app/admin/templates/template-types.enum';
import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetTempaltesQuery extends Query<GetTempaltesQuery_Result[]> {
  getUrl(apiPath: ApiPath): string {
    return `${apiPath.templates()}/GetTemplates`;
  }
}

export interface GetTempaltesQuery_Result {
  templateId: string;
  template: string;
  templateType: TemplateTypes;
  courses: GetTempaltesQuery_Result_Course[];
}

export interface GetTempaltesQuery_Result_Course {
  courseId: string;
  name: string;
}
