import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetLearningMaterialUserAssignmentQuery extends Query<GetLearningMaterialUserAssignmentQuery_Result> {
  constructor(
    public readonly learningMaterialUserAssignmentId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.learningMaterialAssignments()}/GetLearningMaterialUserAssignment`;
  }
}


export interface GetLearningMaterialUserAssignmentQuery_Result {
  readonly learningMaterialUserAssignmentId: string;
  readonly learningMaterialAssignmentId: string;
  readonly contentTreeNodeId: string;
  readonly assignedToUserId: string;
  readonly schoolClassId: string,
  readonly schoolId: string,
  readonly schoolName: string,
  readonly completedAtUtc: Date | undefined | null;
  readonly dueAtUtc: Date;
  readonly dueAtLocal: Date;
  readonly assignedAtUtc: Date;
  readonly assignedAtLocal: Date;
}
