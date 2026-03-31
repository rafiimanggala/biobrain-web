import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { Query } from '../common/query';
import {
  QuizAnalyticOutput_AverageScoreData,
  QuizAnalyticOutput_CourseQuizInfo,
  QuizAnalyticOutput_ProgressData,
  QuizAnalyticOutput_QuizAssignment,
  QuizAnalyticOutput_QuizStudentAssignment,
  QuizAnalyticOutput_Student,
  QuizAnalyticOutput_SubjectInfo,
} from './quiz-analytic-output.models';

export class GetClassResultsQuery extends Command<GetClassResultsQuery_Result> {
  constructor(public readonly schoolClassId: string, public readonly courseId: string, public readonly selectedFilterNodes: string[]) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizzesAnalytic()}/GetClassResults`;
  }
}

export interface GetClassResultsQuery_Result {
  readonly classData: QuizAnalyticOutput_CourseQuizInfo;
  readonly subjectData: QuizAnalyticOutput_SubjectInfo;
  readonly quizAssignments: QuizAnalyticOutput_QuizAssignment[];
  readonly quizStudentAssignments: QuizAnalyticOutput_QuizStudentAssignment[];
  readonly students: QuizAnalyticOutput_Student[];
  readonly averageScoreInfo: QuizAnalyticOutput_AverageScoreData[];
  readonly progressInfo: QuizAnalyticOutput_ProgressData[];
  readonly quizResults: ClassResultsQuery_Result_QuizResultData[];
}

export interface ClassResultsQuery_Result_QuizResultData {
  readonly studentId: string;
  readonly quizId: string;
  readonly quizAssignmentId: string;
  readonly notApplicable: boolean;
  readonly score: number | undefined;
}

