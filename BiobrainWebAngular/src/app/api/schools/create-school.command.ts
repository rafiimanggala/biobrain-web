import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { SchoolStatus } from '../enums/school-status.enum';

export class CreateSchoolCommand extends Command<CreateSchoolCommand_Result> {
  constructor(public readonly name: string, public readonly useAccessCodes: boolean, public readonly status: SchoolStatus, public readonly endDateUtc: string|null, public readonly courses: string[]) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/CreateSchool`;
  }

  deserializeResult(obj: CreateSchoolCommand_Result_Object): CreateSchoolCommand_Result {
    return new CreateSchoolCommand_Result(obj.schoolId);
  }
}

export class CreateSchoolCommand_Result {
  constructor(public readonly schoolId: string) {
  }
}

export interface CreateSchoolCommand_Result_Object {
  schoolId: string;
}
