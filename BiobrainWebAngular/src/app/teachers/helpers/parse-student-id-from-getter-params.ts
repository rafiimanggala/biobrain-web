import { ValueGetterParams } from 'ag-grid-community';

export function parseStudentIdFromGetterParams(params: ValueGetterParams): string {
  // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment
  const data = params.node?.data;
  if (!data) {
    throw new Error('params.node?.data is not defined');
  }

  // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
  const studentId: string = data.studentId as string;
  if (!studentId) {
    throw new Error('Unable to parse student id property');
  }

  return studentId;
}
