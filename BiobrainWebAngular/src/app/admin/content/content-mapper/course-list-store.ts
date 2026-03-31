import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GetCoursesListQuery, GetCoursesListQuery_Result } from 'src/app/api/content/get-courses-list.query';
import { Api } from '../../../api/api.service';

@Injectable()
export class CourseListStore extends BindableListStore<GetCoursesListQuery_Result, CourseListStoreCriteria> {
  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api
  ) {
    super();
  }

  protected getItems(criteria: CourseListStoreCriteria): Observable<GetCoursesListQuery_Result[]> {
    const query = new GetCoursesListQuery();

    return this._api.send(query).pipe(
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      })
    );
  }

  protected getFilterableText(item: GetCoursesListQuery_Result): string {
    return item.name;
  }

  protected itemEqual(x: GetCoursesListQuery_Result, z: GetCoursesListQuery_Result): boolean {
    return x.courseId === z.courseId;
  }
}

export interface CourseListStoreCriteria {
}
