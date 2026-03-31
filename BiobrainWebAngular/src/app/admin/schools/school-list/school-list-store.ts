import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Api } from '../../../api/api.service';
import { GetSchoolListQuery, SchoolListModel_Result } from '../../../api/schools/get-school-list.query';

@Injectable()
export class SchoolListStore extends BindableListStore<SchoolListModel_Result, unknown> {
  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api
  ) {
    super();
  }

  protected getItems(): Observable<SchoolListModel_Result[]> {
    return this._api.send(new GetSchoolListQuery()).pipe(
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      })
    );
  }

  protected getFilterableText(item: SchoolListModel_Result): string {
    return item.name;
  }

  protected itemEqual(x: SchoolListModel_Result, z: SchoolListModel_Result): boolean {
    return x.schoolId === z.schoolId;
  }
}
