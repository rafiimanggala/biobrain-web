import { QuizAnalyticOutput_ProgressData } from 'src/app/api/quizzes/quiz-analytic-output.models';
import { ValueGetterParams } from 'ag-grid-community';
import { parseStudentIdFromGetterParams } from './parse-student-id-from-getter-params';

export function getStudentProgress(
  progressData: QuizAnalyticOutput_ProgressData[]): (params: ValueGetterParams) => number {
  return params => {
    const studentId = parseStudentIdFromGetterParams(params);

    const progressInfo = progressData.find(x => x.studentId === studentId);
    if (!progressInfo) {
      return Number.NaN;
    }

    return progressInfo.progress;
  };
}
