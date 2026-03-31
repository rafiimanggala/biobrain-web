import { Injectable } from '@angular/core';
import { combineLatest, Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import { GetCoursesForStudentQuery } from '../../../api/courses/get-courses-for-student.query';
import { toNonNullable } from '../../../share/helpers/to-non-nullable';

import { CoursesService } from './courses.service';
import { StudentCourse } from './student-course';
import { StudentCourseGroup } from './student-course-group';

@Injectable({
  providedIn: 'root',
})
export class StudentCoursesService {
  constructor(
    private readonly _api: Api,
    private readonly _coursesService: CoursesService,
  ) {
  }

  public getStudentCourses(studentId: string): Observable<StudentCourseGroup[]> {
    return this._getStudentCourses(studentId);
  }

  private _getStudentCourses(studentId: string): Observable<StudentCourseGroup[]> {
    return combineLatest([
      this._api.send(new GetCoursesForStudentQuery(studentId, new Date())),
      this._coursesService.index$,
    ]).pipe(
      map(([studentCourses, courses]) =>
        studentCourses
          .map(c => new StudentCourseGroup(
            c.studentId,
            c.schoolId,
            c.schoolName,
            c.courses.map(x => new StudentCourse(x.courseId, x.courseName, toNonNullable(courses.get(x.courseId)), x.classId, x.className, x.classYear, x.streak)).sort((a, b) => a.compareBySubject(b))
          ))
          .sort((a, b) => a.schoolName.localeCompare(b.schoolName)),
      ),
      catchError(_ => [])
    );
  }
}
