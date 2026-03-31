import { Moment } from 'moment';
import { SchoolStatus } from 'src/app/api/enums/school-status.enum';
import { SchoolDialogDataSettings } from './school-dialog-data-settings';

export class SchoolDialogData {
  name: string | undefined;
  status: SchoolStatus | undefined;
  teachersLicensesNumber: number | undefined;
  studentsLicensesNumber: number | undefined;  
  coursesIds: string[] = [];
  useAccessCodes: boolean = false;
  endDate: Moment | undefined;

  constructor(public readonly settings: SchoolDialogDataSettings) {}
}
