import { Quiz } from './content-data-models';
import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetQuizByIdQuery extends Query<Quiz> {
  constructor(public readonly quizId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.content()}/GetQuizById`;
  }
}
