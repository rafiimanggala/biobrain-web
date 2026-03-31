import { GetTeacherListItemsQuery_Result } from 'src/app/api/teachers/get-teacher-list-items.query';
import { Injectable } from '@angular/core';
import { MultipleSelectStore } from 'src/app/core/stores/multiple-select-store';

@Injectable()
export class TeacherlListItemsMultipleSelectStore extends MultipleSelectStore<GetTeacherListItemsQuery_Result, string> {
  protected getItemValue(item: GetTeacherListItemsQuery_Result): string {
    return item.teacherId;
  }
}
