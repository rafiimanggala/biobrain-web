import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Api } from '../../../api/api.service';
import { GetClassStudentsListQuery, GetClassStudentsListQuery_Result } from 'src/app/api/students/get-class-students-list.query';

@Injectable()
export class ClassStudentListStore extends BindableListStore<GetClassStudentsListQuery_Result, ClassStudentListStoreCriteria> {
  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api
  ) {
    super();
  }

  protected getItems(criteria: ClassStudentListStoreCriteria): Observable<GetClassStudentsListQuery_Result[]> {
    const query = new GetClassStudentsListQuery(criteria.schoolId, criteria.schoolClassId);

    return this._api.send(query).pipe(
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      })
    );
  }

  protected getFilterableText(item: GetClassStudentsListQuery_Result): string {
    return item.fullName;
  }

  protected itemEqual(x: GetClassStudentsListQuery_Result, z: GetClassStudentsListQuery_Result): boolean {
    return x.studentId === z.studentId;
  }
}

export interface ClassStudentListStoreCriteria {
  schoolId: string;
  schoolClassId: string;
}
