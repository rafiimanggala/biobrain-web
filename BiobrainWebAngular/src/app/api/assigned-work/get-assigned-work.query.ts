import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetAssignedWorkQuery extends Query<AssignedWork> {
  constructor(
    public readonly userId: string,
    public readonly courseId: string,
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.assignedWork()}/GetAssignedWork`;
  }
}

export interface AssignedWork {
  readonly quizzes: ActiveQuizAssignment[];
  readonly learningMaterials: ActiveLearningMaterialAssignment[];
}

export interface ActiveQuizAssignment {
  readonly quizStudentAssignmentId: string;
  readonly nodeId: string;
  readonly path: string[];
  readonly nameLines: string[];
  readonly dueAt: Date;
  readonly assignedAt: Date;
}

export interface ActiveLearningMaterialAssignment {
  readonly learningMaterialUserAssignmentId: string;
  readonly nodeId: string;
  readonly path: string[];
  readonly nameLines: string[];
  readonly dueAt: Date;
  readonly assignedAt: Date;
}
