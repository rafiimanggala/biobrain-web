import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import {
  GetContentTreeMetaListQuery,
  GetContentTreeMetaListQuery_Result
} from 'src/app/api/content/get-content-tree-meta-list.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';

import { Api } from '../../../api/api.service';

@Injectable()
export class ContentTreeMetaListStore extends BindableListStore<GetContentTreeMetaListQuery_Result, ContentTreeMetaListStoreCriteria> {


  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api
  ) {
    super();
  }

  protected getItems(criteria: ContentTreeMetaListStoreCriteria): Observable<GetContentTreeMetaListQuery_Result[]> {
    const query = new GetContentTreeMetaListQuery(criteria.courseId);

    return this._api.send(query).pipe(
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      })
    );
  }

  protected getFilterableText(item: GetContentTreeMetaListQuery_Result): string {
    return item.name;
  }

  protected itemEqual(x: GetContentTreeMetaListQuery_Result, z: GetContentTreeMetaListQuery_Result): boolean {
    return x.contentTreeMetaId === z.contentTreeMetaId;
  }
}

export interface ContentTreeMetaListStoreCriteria {
  courseId: string;
}
