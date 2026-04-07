import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetStudentCustomQuizzesQuery extends Query<GetStudentCustomQuizzesQuery_Result> {
  constructor(public readonly userId: string, public readonly courseId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizAssignments()}/GetStudentCustomQuizzes`;
  }
}

export interface GetStudentCustomQuizzesQuery_Result {
  readonly quizzes: GetStudentCustomQuizzesQuery_Item[];
}

export interface GetStudentCustomQuizzesQuery_Item {
  readonly quizId: string;
  readonly name: string;
  readonly questionCount: number;
  readonly createdAt: string;
  readonly contentTreeNodeIds: string[];
}
