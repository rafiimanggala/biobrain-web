import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetQuizTemplatesQuery extends Query<GetQuizTemplatesQuery_Result> {
  constructor(
    public readonly teacherId: string,
    public readonly courseId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/GetQuizTemplates`;
  }
}

export interface GetQuizTemplatesQuery_Result {
  readonly templates: QuizTemplate[];
}

export interface QuizTemplate {
  readonly templateId: string;
  readonly name: string;
  readonly questionCount: number;
  readonly courseId: string;
  readonly createdAt: Date;
}
