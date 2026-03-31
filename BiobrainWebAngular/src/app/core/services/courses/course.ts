import { Curriculum } from '../curricula/curriculum';
import { Subject } from '../subjects/subject';

export class Course {
  constructor(
    public courseId: string,
    public subjectCode: number,
    public curriculumCode: number,
    public subject: Subject,
    public curriculum: Curriculum,
    public year: number,
    public displayName: string,
    public subHeader: string | null
  ) {
  }
}
