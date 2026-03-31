import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  GetTeacherListItemsQuery,
  GetTeacherListItemsQuery_Result,
} from 'src/app/api/teachers/get-teacher-list-items.query';
import { BindableFilterableListStore } from 'src/app/core/stores/bindable-filterable-list-store';
import { ListItem } from 'src/app/core/stores/list-item';

import { Api } from '../../api/api.service';

@Injectable()
export class TeacherListItemsStore extends BindableFilterableListStore<GetTeacherListItemsQuery_Result, Criteria> {
  readonly availableListItems$ = this.availableItems$.pipe(
    map(items => items.map(item => this.toListItem(item))),
  );

  readonly allListItems$ = this.items$.pipe(
    map(items => items.map(item => this.toListItem(item))),
  );

  constructor(private readonly _api: Api) {
    super();
  }

  protected getItems(criteria: Criteria): Observable<GetTeacherListItemsQuery_Result[]> {
    return this._api.send(new GetTeacherListItemsQuery(criteria.schoolId, ''));
  }

  protected toListItem(item: GetTeacherListItemsQuery_Result): ListItem<string> {
    return { value: item.teacherId, text: item.fullName };
  }

  protected getFilterableText(item: GetTeacherListItemsQuery_Result): string {
    return item.fullName;
  }

  protected itemEqual(x: GetTeacherListItemsQuery_Result, z: GetTeacherListItemsQuery_Result): boolean {
    return x.teacherId === z.teacherId;
  }
}

export interface Criteria {
  schoolId: string;
}
