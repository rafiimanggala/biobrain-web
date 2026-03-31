import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';
import { AssignmentStatus } from '../enums/assignment-status.enum';

export class GetTeacherAssignedWorkQuery extends Query<TeacherAssignedWork> {
  constructor(
    public readonly userId: string,
    public readonly schoolClassId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.assignedWork()}/GetTeacherAssignedWork`;
  }
}

export interface TeacherAssignedWork {
  readonly quizzes: ActiveQuizClassAssignment[];
  readonly learningMaterials: ActiveLearningMaterialClassAssignment[];
}

export interface ActiveQuizClassAssignment {
  readonly quizAssignmentId: string;
  readonly nodeId: string;
  readonly path: string[];
  readonly title: string,
  readonly dueAt: Date;
  readonly assignedAt: Date;
  readonly studentAssigned: number;
  readonly status: AssignmentStatus;
}

export interface ActiveLearningMaterialClassAssignment {
  readonly learningMaterialAssignmentId: string;
  readonly nodeId: string;
  readonly path: string[];
  readonly title: string,
  readonly dueAt: Date;
  readonly assignedAt: Date;
  readonly studentAssigned: number;
  readonly status: AssignmentStatus;
}
