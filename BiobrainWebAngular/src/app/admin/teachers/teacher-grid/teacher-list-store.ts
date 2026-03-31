import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { GetSchoolTeachersListQuery, GetSchoolTeachersListQuery_Result } from 'src/app/api/teachers/get-school-teachers-list.query';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Api } from '../../../api/api.service';

@Injectable()
export class TeacherListStore extends BindableListStore<GetSchoolTeachersListQuery_Result, TeacherListStoreCriteria> {
  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api
  ) {
    super();
  }

  protected getItems(criteria: TeacherListStoreCriteria): Observable<GetSchoolTeachersListQuery_Result[]> {
    const query = new GetSchoolTeachersListQuery(criteria.schoolId);

    return this._api.send(query).pipe(
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      })
    );
  }

  protected getFilterableText(item: GetSchoolTeachersListQuery_Result): string {
    return item.fullName;
  }

  protected itemEqual(x: GetSchoolTeachersListQuery_Result, z: GetSchoolTeachersListQuery_Result): boolean {
    return x.teacherId === z.teacherId;
  }
}

export interface TeacherListStoreCriteria {
  schoolId: string;
}
