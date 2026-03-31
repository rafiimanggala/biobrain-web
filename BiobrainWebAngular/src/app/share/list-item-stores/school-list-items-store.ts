import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  GetSchoolListItemsQuery,
  GetSchoolListItemsQuery_Result,
} from 'src/app/api/schools/get-school-list-items.query';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { ListItem } from 'src/app/core/stores/list-item';

import { Api } from '../../api/api.service';

@Injectable()
export class SchoolListItemsStore extends BindableListStore<GetSchoolListItemsQuery_Result, unknown> {
  public listItems$ = this.items$.pipe(
    map(items => items.map(item => this.toListItem(item))),
  );

  constructor(private readonly _api: Api) {
    super();
  }

  protected getItems(
    _criteria: unknown
  ): Observable<GetSchoolListItemsQuery_Result[]> {
    return this._api.send(new GetSchoolListItemsQuery());
  }

  protected toListItem(item: GetSchoolListItemsQuery_Result): ListItem<string> {
    return { value: item.schoolId, text: item.name };
  }
}
