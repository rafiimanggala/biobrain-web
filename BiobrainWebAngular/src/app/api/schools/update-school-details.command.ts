import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';
import { SchoolStatus } from '../enums/school-status.enum';

export class UpdateSchoolDetailsCommand extends Command<UpdateSchoolDetailsCommand_Result> {
  constructor(public readonly schoolId: string, public readonly name: string, public readonly useAccessCodes: boolean, public readonly status: SchoolStatus, public readonly endDateUtc: string|null, public readonly courses: string[]) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schools()}/UpdateSchoolDetails`;
  }

  deserializeResult(obj: UpdateSchoolDetailsCommand_Result_Object): UpdateSchoolDetailsCommand_Result {
    return new UpdateSchoolDetailsCommand_Result(obj.schoolId, obj.name, obj.status);
  }
}

export class UpdateSchoolDetailsCommand_Result {
  constructor(public readonly schoolId: string, public readonly name: string, public readonly status: SchoolStatus) {
  }
}

export interface UpdateSchoolDetailsCommand_Result_Object {
  schoolId: string;
  name: string;
  status: SchoolStatus;
}
