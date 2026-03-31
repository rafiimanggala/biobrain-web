import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  GetStudentListItemsQuery,
  GetStudentListItemsQuery_Result,
} from 'src/app/api/students/get-student-list-items.query';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { ListItem } from 'src/app/core/stores/list-item';

import { Api } from '../../api/api.service';

@Injectable()
export class StudentListItemsStore extends BindableListStore<GetStudentListItemsQuery_Result, Criteria> {
  public listItems$ = this.items$.pipe(
    map(items => items.map(item => this.toListItem(item))),
  );

  constructor(private readonly _api: Api) {
    super();
  }

  protected getItems(
    criteria: Criteria
  ): Observable<GetStudentListItemsQuery_Result[]> {
    return this._api.send(
      new GetStudentListItemsQuery(criteria.schoolId)
    );
  }

  protected toListItem(
    item: GetStudentListItemsQuery_Result
  ): ListItem<string> {
    return { value: item.studentId, text: item.fullName };
  }

  protected getFilterableText(item: GetStudentListItemsQuery_Result): string {
    return item.studentId;
  }

  protected itemEqual(x: GetStudentListItemsQuery_Result, z: GetStudentListItemsQuery_Result): boolean {
    return x.studentId === z.studentId;
  }
}

export interface Criteria {
  schoolId: string;
}
