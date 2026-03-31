import { Moment } from 'moment';

export class AssignLearningMaterialsAndQuizzesDialogResult {
  constructor(
    public readonly studentIdsBySchoolClassIdMap: Record<string, string[]>,
    public readonly dueDate: Moment,
    public readonly hintsEnabled: boolean,
    public readonly soundEnabled: boolean,
    public readonly selectedLearningMaterialIds: string[],
  ) {
  }
}
