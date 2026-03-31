import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

import {
  QuizAnalyticOutput_CourseQuizInfo,
  QuizAnalyticOutput_Student,
  QuizAnalyticOutput_SubjectInfo,
} from './quiz-analytic-output.models';

export class GetStudentQuizAssignmentResultsQuery extends Query<GetStudentQuizAssignmentResultsQuery_Result> {
  constructor(
    public readonly studentId: string,
    public readonly schoolClassId: string,
    public readonly courseId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizzesAnalytic()}/GetStudentQuizAssignmentResults`;
  }
}

export interface GetStudentQuizAssignmentResultsQuery_Result {
  readonly studentInfo: QuizAnalyticOutput_Student;
  readonly classData: QuizAnalyticOutput_CourseQuizInfo;
  readonly subjectData: QuizAnalyticOutput_SubjectInfo;
  readonly results: GetStudentQuizAssignmentResultsQuery_Result_QuizAssignmentResult[];
}

export interface GetStudentQuizAssignmentResultsQuery_Result_QuizAssignmentResult {
  readonly quizStudentAssignmentId: string;
  readonly quizAssignmentId: string;
  readonly quizId: string;
  readonly contentTreeNodeId: string;
  readonly score: number;
  readonly progress: number;
  readonly completedAt: string | null | undefined;
  readonly notApplicable: boolean;
  readonly quizNameHtml: string
}
