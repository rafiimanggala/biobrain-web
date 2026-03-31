import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetCourseQuizzesListQuery extends Query<GetCourseQuizzesListQuery_Result[]> {
  constructor(
    public courseId: string
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetCourseQuizzesList`;
  }
}

export interface GetCourseQuizzesListQuery_Result {
  quizId: string;
  name: string;
}
