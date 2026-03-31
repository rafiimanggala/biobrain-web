import { Injectable } from '@angular/core';
import { combineLatest, Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { GetCoursesForTeacherQuery } from 'src/app/api/courses/get-courses-for-teacher.query';

import { Api } from '../../../api/api.service';
import { toNonNullable } from '../../../share/helpers/to-non-nullable';
import { Logger } from '../logger';

import { CoursesService } from './courses.service';
import { TeacherCourse } from './teacher-course';
import { TeacherCourseGroup } from './teacher-course-group';

@Injectable({
  providedIn: 'root',
})
export class TeacherCoursesService {
  constructor(
    private readonly _api: Api,
    private readonly _coursesService: CoursesService,
    private readonly _logger: Logger
  ) {
  }

  public getTeacherCourses(teacherId: string): Observable<TeacherCourseGroup[]> {
    return this._getTeacherCourses(teacherId);
  }

  private _getTeacherCourses(teacherId: string): Observable<TeacherCourseGroup[]> {
    return combineLatest([
      this._api.send(new GetCoursesForTeacherQuery(teacherId)),
      this._coursesService.index$,
    ]).pipe(
      map(([teacherCourses, courses]) =>
        teacherCourses
          .map(c => new TeacherCourseGroup(
            c.teacherId,
            c.schoolId,
            c.schoolName,
            c.courses.map(x => new TeacherCourse(x.courseId, toNonNullable(courses.get(x.courseId)), x.classId, x.className, x.classYear)).sort((a, b) => a.compareBySubject(b))
          ))
          .sort((a, b) => a.schoolName.localeCompare(b.schoolName)),
      ),
      catchError(_ => { this._logger.logError(JSON.stringify(_)); return [];})
    );
  }
}
