import { ValueGetterParams } from 'ag-grid-community';
import { QuizAnalyticOutput_Student } from 'src/app/api/quizzes/quiz-analytic-output.models';
import { Student } from 'src/app/core/services/students/student';

import { parseStudentIdFromGetterParams } from './parse-student-id-from-getter-params';

export function getStudentNameGetter(students: QuizAnalyticOutput_Student[] | Student[]): (params: ValueGetterParams) => string {
  return params => {
    const studentId = parseStudentIdFromGetterParams(params);

    const student = students.find(x => x.studentId === studentId);
    if (!student) {
      throw new Error(`Student with id = ${studentId} not found`);
    }

    return `${student.firstName} ${student.lastName}`;
  };
}
