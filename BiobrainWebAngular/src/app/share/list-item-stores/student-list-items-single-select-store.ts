import { GetStudentListItemsQuery_Result } from 'src/app/api/students/get-student-list-items.query';
import { Injectable } from '@angular/core';
import { SingleSelectStore } from 'src/app/core/stores/single-select-store';

@Injectable()
export class StudentlListItemsSingleSelectStore extends SingleSelectStore<GetStudentListItemsQuery_Result, string> {
  protected getItemValue(item: GetStudentListItemsQuery_Result): string {
    return item.studentId;
  }
}
