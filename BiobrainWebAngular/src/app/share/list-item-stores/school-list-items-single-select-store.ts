import { GetSchoolListItemsQuery_Result } from 'src/app/api/schools/get-school-list-items.query';
import { Injectable } from '@angular/core';
import { SingleSelectStore } from 'src/app/core/stores/single-select-store';

@Injectable()
export class SchoolListItemsSingleSelectStore extends SingleSelectStore<GetSchoolListItemsQuery_Result, string> {
  protected getItemValue(item: GetSchoolListItemsQuery_Result): string {
    return item.schoolId;
  }
}
