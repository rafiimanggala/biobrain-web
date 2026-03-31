import { StudentCourse } from './student-course';

export class StudentCourseGroup {
  constructor(
    public readonly studentId: string,
    public readonly schoolId: string,
    public readonly schoolName: string,
    public readonly courses: StudentCourse[]
  ) {
  }
}
