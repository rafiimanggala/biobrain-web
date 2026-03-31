import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class UpdateSchoolLicensesCommand extends Command<UpdateSchoolLicensesCommand_Result> {
  constructor(
    public readonly schoolId: string,
    public readonly teachersLicensesNumber: number,
    public readonly studentsLicensesNumber: number
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/UpdateSchoolLicenses`;
  }
}

export class UpdateSchoolLicensesCommand_Result {
}
