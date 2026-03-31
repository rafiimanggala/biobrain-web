import { GetTeacherListItemsQuery_Result } from 'src/app/api/teachers/get-teacher-list-items.query';
import { Injectable } from '@angular/core';
import { SingleSelectStore } from 'src/app/core/stores/single-select-store';

@Injectable()
export class TeacherlListItemsSingleSelectStore extends SingleSelectStore<GetTeacherListItemsQuery_Result, string> {
  protected getItemValue(item: GetTeacherListItemsQuery_Result): string {
    return item.teacherId;
  }
}
