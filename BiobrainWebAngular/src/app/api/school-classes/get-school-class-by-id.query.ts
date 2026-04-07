import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetSchoolClassByIdQuery extends Query<GetSchoolClassByIdQuery_Result> {
  constructor(public readonly schoolClassId: string) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.schoolClasses()}/GetById`;
  }

  deserializeResult(obj: GetSchoolClassByIdQuery_Result_Object): GetSchoolClassByIdQuery_Result {
    return new GetSchoolClassByIdQuery_Result(
      obj.schoolClassId,
      obj.schoolId,
      obj.courseId,
      obj.year,
      obj.name,
      obj.autoJoinClassCode,
      obj.hintsDisabled ?? false,
      obj.soundDisabled ?? false,
      obj.teacherIds,
      obj.students.map(x => new GetSchoolClassByIdQuery_Student_Result(x.studentId, x.email))
    );
  }
}

export class GetSchoolClassByIdQuery_Result {
  constructor(
    public readonly schoolClassId: string,
    public readonly schoolId: string,
    public readonly courseId: string,
    public readonly year: number,
    public readonly name: string,
    public readonly autoJoinClassCode: string,
    public readonly hintsDisabled: boolean,
    public readonly soundDisabled: boolean,
    public readonly teacherIds: string[],
    public readonly students: GetSchoolClassByIdQuery_Student_Result[]
  ) {
  }
}

export class GetSchoolClassByIdQuery_Student_Result{
  constructor(
    public readonly studentId: string,
    public readonly email: string,
    ){}
}

export interface GetSchoolClassByIdQuery_Result_Object {
  schoolClassId: string;
  schoolId: string;
  courseId: string;
  year: number;
  name: string;
  autoJoinClassCode: string,
  hintsDisabled?: boolean,
  soundDisabled?: boolean,
  teacherIds: string[],
  students: GetSchoolClassByIdQuery_Student_Result_Object[]
}

export interface GetSchoolClassByIdQuery_Student_Result_Object{
  studentId: string;
  email: string;
}
