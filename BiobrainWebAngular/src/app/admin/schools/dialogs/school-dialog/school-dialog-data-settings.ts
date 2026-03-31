
export class SchoolDialogDataSettings {
  constructor(readonly forbiddenNames: Set<string>, readonly courses: {courseId: string, name: string, isSelected: boolean}[]) { }
}
