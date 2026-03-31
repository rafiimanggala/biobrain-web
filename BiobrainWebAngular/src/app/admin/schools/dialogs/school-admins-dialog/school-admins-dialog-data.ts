import { SchoolAdminsDialogSettings } from './school-admins-dialog-settings';

export class SchoolAdminsDialogData {
  constructor(
    public teachers: string[] | null | undefined,
    public readonly settings: SchoolAdminsDialogSettings
  ) {}
}
