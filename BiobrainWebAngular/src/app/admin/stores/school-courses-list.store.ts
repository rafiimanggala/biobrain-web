import { Injectable } from '@angular/core';
import { combineLatest, Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { GetCoursesForSchoolQuery } from 'src/app/api/courses/get-courses-for-school.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { CurriculaService } from 'src/app/core/services/curricula/curricula.service';
import { SubjectsService } from 'src/app/core/services/subjects/subjects.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';

import { Api } from '../../api/api.service';

@Injectable()
export class SchoolCoursesListStore extends BindableListStore<SchoolCoursesListStoreItem, SchoolCoursesListStoreCriteria> {
  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api,
    private readonly _subjectService: SubjectsService,
    private readonly _curriculumService: CurriculaService,
  ) {
    super();
  }

  protected getItems(criteria: SchoolCoursesListStoreCriteria): Observable<SchoolCoursesListStoreItem[]> {
    return combineLatest([
      this._api.send(new GetCoursesForSchoolQuery(criteria.schoolId)),
      this._subjectService.index$,
      this._curriculumService.index$,
    ]).pipe(
      map(([schoolCourses, subjects, curricula]) =>
        schoolCourses.map(course => {
          const subject = subjects.get(course.subjectCode);
          const curriculum = curricula.get(course.curriculumCode);
          return {
            courseId: course.courseId,
            subjectCode: course.subjectCode,
            curriculumCode: course.curriculumCode,
            subjectName: subject?.name ?? '',
            curriculumName: curriculum?.name ?? '',
          };
        }),
      ),
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      }),
    );
  }

  protected getFilterableText(item: SchoolCoursesListStoreItem): string {
    return item.subjectName + ' ' + item.curriculumName;
  }

  protected itemEqual(x: SchoolCoursesListStoreItem, z: SchoolCoursesListStoreItem): boolean {
    return x.courseId === z.courseId;
  }
}

export interface SchoolCoursesListStoreItem {
  courseId: string;
  subjectCode: number;
  curriculumCode: number;
  subjectName: string;
  curriculumName: string;
}

export interface SchoolCoursesListStoreCriteria {
  schoolId: string;
}
