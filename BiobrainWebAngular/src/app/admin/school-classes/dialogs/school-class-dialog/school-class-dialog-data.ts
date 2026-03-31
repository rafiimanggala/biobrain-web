export interface SchoolClassDialogData {
  year: number | null | undefined;
  name: string | null | undefined;
  courseId: string | null | undefined;
  settings: SchoolClassDialogSettings;
  teacherIds: string[];
}


export class SchoolClassDialogSettings {
  constructor(
    public readonly schoolId: string,
    public readonly currentClassId: string | undefined
  ) {}
}


export function isValid(data: SchoolClassDialogData): boolean {
  if (!data.name) {
    return false;
  }

  if (!data.courseId) {
    return false;
  }

  if (data.year === null || data.year === undefined) {
    return false;
  }

  if (data.year <= 0) {
    return false;
  }

  // TODO: validate upper bound of year.

  return true;
}
