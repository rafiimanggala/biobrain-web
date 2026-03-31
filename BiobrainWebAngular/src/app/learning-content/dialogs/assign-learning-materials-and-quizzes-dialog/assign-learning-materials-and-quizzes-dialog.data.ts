import { hasValue } from '../../../share/helpers/has-value';

export class AssignLearningMaterialsAndQuizzesDialogData {
  public readonly schoolClassIds: string[] | undefined;

  constructor(
    public readonly title: string,
    public readonly learningMaterialIds: string[],
    public readonly quizIds: string[],
    public readonly schoolId: string,
    schoolClassIds: string[] | undefined,
    public readonly studentIdList: string[] = [],
  ) {
    this.schoolClassIds = hasValue(schoolClassIds)
      ? [...new Set(schoolClassIds.map(x => x.trim().toLowerCase()))]
      : undefined;
  }
}
