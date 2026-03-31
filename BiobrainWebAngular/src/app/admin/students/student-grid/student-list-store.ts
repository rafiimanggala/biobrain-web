import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Api } from '../../../api/api.service';
import {
  GetSchoolStudentsListQuery,
  GetSchoolStudentsListQuery_Result
} from '../../../api/students/get-school-students-list.query';

@Injectable()
export class StudentListStore extends BindableListStore<GetSchoolStudentsListQuery_Result, StudentListStoreCriteria> {
  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api
  ) {
    super();
  }

  protected getItems(criteria: StudentListStoreCriteria): Observable<GetSchoolStudentsListQuery_Result[]> {
    const query = new GetSchoolStudentsListQuery(criteria.schoolId);

    return this._api.send(query).pipe(
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      })
    );
  }

  protected getFilterableText(item: GetSchoolStudentsListQuery_Result): string {
    return item.fullName;
  }

  protected itemEqual(x: GetSchoolStudentsListQuery_Result, z: GetSchoolStudentsListQuery_Result): boolean {
    return x.studentId === z.studentId;
  }
}

export interface StudentListStoreCriteria {
  schoolId: string;
  schoolClassId: string | null;
}
