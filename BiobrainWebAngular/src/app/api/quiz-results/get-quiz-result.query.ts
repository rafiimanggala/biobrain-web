import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetQuizResultQuery extends Query<GetQuizResultQuery_Result> {
  constructor(public readonly quizResultId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/GetQuizResult`;
  }
}

export interface GetQuizResultQuery_Result {
  quizResultId: string;
  quizId: string;
  userId: string;
  readonly schoolClassId: string;
  readonly schoolId: string;
  readonly schoolName: string;
  score: number;
  questions: GetQuizResultQuery_ResultQuestion[];
  excludedQuestions: string[];
  hintsEnabled?: boolean;
  soundEnabled?: boolean;
}

export interface GetQuizResultQuery_ResultQuestion {
  quizResultId: string;
  questionId: string;
  value: string;
  isCorrect: boolean;
}
