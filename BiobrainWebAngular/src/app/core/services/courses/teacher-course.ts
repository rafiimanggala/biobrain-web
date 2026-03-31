import { Course } from './course';

export class TeacherCourse {
  constructor(
    public readonly courseId: string,
    public readonly course: Course,
    public readonly classId: string,
    public readonly className: string,
    public readonly classYear: number,
  ) {
  }

  public compareBySubject(b: TeacherCourse): number {
    if (this.course.subjectCode > b.course.subjectCode) return 1;
    if (this.course.subjectCode < b.course.subjectCode) return -1;
    return this.className.localeCompare(b.className);
  }
}
