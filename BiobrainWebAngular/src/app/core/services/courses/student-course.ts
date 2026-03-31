import { Course } from './course';

export class StudentCourse {
  constructor(
    public readonly courseId: string,
    public readonly courseName: string,
    public readonly course: Course,
    public readonly classId: string,
    public readonly className: string,
    public readonly classYear: number,
    public readonly streak: number
  ) {
    this.displayName = className ?? courseName;
  }
  public readonly displayName: string;

  public compareBySubject(b: StudentCourse): number {
    return this.course.subjectCode - b.course.subjectCode;
  }
}
