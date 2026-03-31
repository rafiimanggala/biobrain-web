import { TeacherCourseGroup } from '../../../core/services/courses/teacher-course-group';

export class SelectClassDialogData {
  constructor(
    public readonly courseGroups: TeacherCourseGroup[],
  ) {
  }
}
