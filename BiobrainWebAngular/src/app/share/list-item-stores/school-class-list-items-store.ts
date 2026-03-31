import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  GetSchoolClassListItemsQuery,
  GetSchoolClassListItemsQuery_Result,
} from 'src/app/api/school-classes/get-school-class-list-items.query';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { ListItem } from 'src/app/core/stores/list-item';

import { Api } from '../../api/api.service';

@Injectable()
export class SchoolClassListItemsStore extends BindableListStore<GetSchoolClassListItemsQuery_Result, Criteria> {
  public listItems$ = this.items$.pipe(
    map(items => items.map(item => this.toListItem(item))),
  );

  constructor(private readonly _api: Api) {
    super();
  }

  protected getItems(criteria: Criteria): Observable<GetSchoolClassListItemsQuery_Result[]> {
    return this._api.send(new GetSchoolClassListItemsQuery(criteria.schoolId));
  }

  protected toListItem(item: GetSchoolClassListItemsQuery_Result): ListItem<string> {
    return { value: item.schoolClassId, text: item.name };
  }

  protected getFilterableText(item: GetSchoolClassListItemsQuery_Result): string {
    return item.name;
  }

  protected itemEqual(x: GetSchoolClassListItemsQuery_Result, z: GetSchoolClassListItemsQuery_Result): boolean {
    return x.schoolClassId === z.schoolClassId;
  }
}

export interface Criteria {
  schoolId: string;
}
