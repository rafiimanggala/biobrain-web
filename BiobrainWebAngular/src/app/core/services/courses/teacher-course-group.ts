import { TeacherCourse } from './teacher-course';

export class TeacherCourseGroup {
  constructor(
    public readonly teacherId: string,
    public readonly schoolId: string,
    public readonly schoolName: string,
    public readonly courses: TeacherCourse[]
  ) {
  }
}
