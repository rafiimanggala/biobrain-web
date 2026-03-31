import { Injectable } from '@angular/core';
import { combineLatest, Observable } from 'rxjs';
import { map, shareReplay } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import {
  GetCoursesForSchoolQuery,
  GetCoursesForSchoolQuery_Result,
} from '../../../api/courses/get-courses-for-school.query';
import { GetCoursesQuery } from '../../../api/courses/get-courses.query';
import { toDictionary } from '../../../share/helpers/observable-operators';
import { toNonNullable } from '../../../share/helpers/to-non-nullable';
import { CurriculaService } from '../curricula/curricula.service';
import { SubjectsService } from '../subjects/subjects.service';

import { Course } from './course';

@Injectable({
  providedIn: 'root',
})
export class CoursesService {
  public readonly items$: Observable<Course[]>;
  public readonly index$: Observable<Map<string, Course>>;

  constructor(
    private readonly _api: Api,
    private readonly _subjectsService: SubjectsService,
    private readonly _curriculaService: CurriculaService,
  ) {
    this.items$ = combineLatest([
      this._api.send(new GetCoursesQuery()),
      this._subjectsService.index$,
      this._curriculaService.index$,
    ]).pipe(
      map(([items, subjects, curricula]) =>
        items.map(_ => new Course(
          _.courseId,
          _.subjectCode,
          _.curriculumCode,
          toNonNullable(subjects.get(_.subjectCode)),
          toNonNullable(curricula.get(_.curriculumCode)),
          _.year,
          _.name,
          _.subHeader
        )),
      ),
      shareReplay(1),
    );

    this.index$ = this.items$.pipe(toDictionary(_ => _.courseId), shareReplay(1));
  }

  public getCoursesForSchool(schoolId: string): Observable<GetCoursesForSchoolQuery_Result[]> {
    return this._api.send(new GetCoursesForSchoolQuery(schoolId)).pipe(
      shareReplay(1),
    );
  }

  findById(courseId: string): Observable<Course | undefined> {
    return this.index$.pipe(map(index => index.get(courseId)));
  }
}
