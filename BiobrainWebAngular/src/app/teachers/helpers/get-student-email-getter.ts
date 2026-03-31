import { ValueGetterParams } from 'ag-grid-community';
import { Student } from 'src/app/core/services/students/student';

import { parseStudentIdFromGetterParams } from './parse-student-id-from-getter-params';

export function getStudentEmailGetter(students: Student[]): (params: ValueGetterParams) => string {
  return params => {
    const studentId = parseStudentIdFromGetterParams(params);

    const student = students.find(x => x.studentId === studentId);
    if (!student) {
      throw new Error(`Student with id = ${studentId} not found`);
    }

    return student.email;
  };
}
