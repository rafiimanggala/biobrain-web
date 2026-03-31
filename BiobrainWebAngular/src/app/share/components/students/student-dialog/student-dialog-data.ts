export interface StudentDialogData {
  email: string | null | undefined;
  firstName: string | null | undefined;
  lastName: string | null | undefined;
  country: string;
  state: string | null | undefined;
  curriculumCode: number | null | undefined;
  year: number | null | undefined;

  settings: StudentDialogDataSettings;
}

export class StudentDialogDataSettings {
  constructor(public readonly isEditMode: boolean) {}
}

export function isValid(data: StudentDialogData): boolean {
  if (!data.firstName) {
    return false;
  }

  if (!data.lastName) {
    return false;
  }

  if (!data.email) {
    return false;
  }

  if (!data.country) {
    return false;
  }

  return true;
}
