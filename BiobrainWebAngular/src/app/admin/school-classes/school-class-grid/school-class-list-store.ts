import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';

import { Api } from '../../../api/api.service';
import {
  GetSchoolClassesListQuery,
  GetSchoolClassesListQuery_Result
} from '../../../api/school-classes/get-school-classes-list.query';

@Injectable()
export class SchoolClassListStore extends BindableListStore<GetSchoolClassesListQuery_Result, SchoolClassListStoreCriteria> {
  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api
  ) {
    super();
  }

  protected getItems(criteria: SchoolClassListStoreCriteria): Observable<GetSchoolClassesListQuery_Result[]> {
    const query = new GetSchoolClassesListQuery(criteria.schoolId);

    return this._api.send(query).pipe(
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      })
    );
  }

  protected getFilterableText(item: GetSchoolClassesListQuery_Result): string {
    return item.name;
  }

  protected itemEqual(x: GetSchoolClassesListQuery_Result, z: GetSchoolClassesListQuery_Result): boolean {
    return x.schoolClassId === z.schoolClassId;
  }
}

export interface SchoolClassListStoreCriteria {
  schoolId: string;
}
