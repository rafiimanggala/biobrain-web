import { SchoolLicensesDialogSettings } from './school-licenses-dialog-settings';

export class SchoolLicensesDialogData {
  constructor(
    public teachersLicensesNumber: number,
    public studentsLicensesNumber: number,
    public readonly settings: SchoolLicensesDialogSettings
  ) {}
}
