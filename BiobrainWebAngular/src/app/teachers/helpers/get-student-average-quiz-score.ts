import { QuizAnalyticOutput_AverageScoreData } from 'src/app/api/quizzes/quiz-analytic-output.models';
import { StringsService } from 'src/app/share/strings.service';
import { ValueGetterParams } from 'ag-grid-community';
import { parseStudentIdFromGetterParams } from './parse-student-id-from-getter-params';

export function getStudentAverageQuizScore(
  averageScoreInfo: QuizAnalyticOutput_AverageScoreData[],
  strings: StringsService): (params: ValueGetterParams) => string {
  return params => {
    const studentId = parseStudentIdFromGetterParams(params);

    const scoreInfo = averageScoreInfo.find(x => x.studentId === studentId);
    if (!scoreInfo) {
      return strings.zeroPercent;
    }

    return scoreInfo.notApplicable
      ? strings.notApplicableShort
      : `${Number(scoreInfo.averageScore.toFixed(0)).toLocaleString()} %`;
  };
}
