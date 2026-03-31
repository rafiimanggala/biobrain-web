import { GetSchoolClassListItemsQuery_Result } from 'src/app/api/school-classes/get-school-class-list-items.query';
import { Injectable } from '@angular/core';
import { SingleSelectStore } from 'src/app/core/stores/single-select-store';

@Injectable()
export class SchoolClasslListItemsSingleSelectStore extends SingleSelectStore<GetSchoolClassListItemsQuery_Result, string> {
  protected getItemValue(item: GetSchoolClassListItemsQuery_Result): string {
    return item.schoolClassId;
  }
}
