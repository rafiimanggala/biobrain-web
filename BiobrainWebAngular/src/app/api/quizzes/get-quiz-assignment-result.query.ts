import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';
import { Answer } from '../content/content-data-models';

import {
  QuizAnalyticOutput_AverageScoreData,
  QuizAnalyticOutput_CourseQuizInfo,
  QuizAnalyticOutput_ProgressData,
  QuizAnalyticOutput_Student,
  QuizAnalyticOutput_SubjectInfo,
} from './quiz-analytic-output.models';

export class GetQuizAssignmentResultQuery extends Query<GetQuizAssignmentResultQuery_Result> {
  constructor(public readonly quizAssignmentId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizzesAnalytic()}/GetQuizAssignmentResult`;
  }
}

export interface GetQuizAssignmentResultQuery_Result {
  readonly contentTreeNodeId: string;
  readonly quizId: string;
  readonly classData: QuizAnalyticOutput_CourseQuizInfo | undefined | null;
  readonly subjectData: QuizAnalyticOutput_SubjectInfo;
  readonly students: QuizAnalyticOutput_Student[];
  readonly averageScoreInfo: QuizAnalyticOutput_AverageScoreData[];
  readonly progressInfo: QuizAnalyticOutput_ProgressData[];
  readonly questions: GetQuizAssignmentResultQuery_Result_Question[];
  readonly questionResults: GetQuizAssignmentResultQuery_Result_QuestionResult[];
  readonly quizName: string;
}

export interface GetQuizAssignmentResultQuery_Result_QuestionResult {
  readonly studentId: string;
  readonly questionId: string;
  readonly isCorrect: boolean;
}

export interface GetQuizAssignmentResultQuery_Result_Question {
  readonly questionId: string;
  text: string;
  readonly header: string;
  readonly questionTypeCode: string;
}
