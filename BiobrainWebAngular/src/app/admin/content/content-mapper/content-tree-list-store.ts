import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BindableListStore } from 'src/app/core/stores/bindable-list-store';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GetContentTreeListQuery, GetContentTreeListQuery_Result } from 'src/app/api/content/get-content-tree-list.query';
import { TreeMode } from 'src/app/api/enums/tree-mode.enum';
import { Api } from '../../../api/api.service';

@Injectable()
export class ContentTreeListStore extends BindableListStore<GetContentTreeListQuery_Result, ContentTreeListStoreCriteria> {
  constructor(
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api
  ) {
    super();
  }

  protected getItems(criteria: ContentTreeListStoreCriteria): Observable<GetContentTreeListQuery_Result[]> {
    const query = new GetContentTreeListQuery(criteria.courseId, TreeMode.All);

    return this._api.send(query).pipe(
      catchError(error => {
        this._appEvents.errorEmit(error);
        throw error;
      })
    );
  }

  protected getFilterableText(item: GetContentTreeListQuery_Result): string {
    return item.header;
  }

  protected itemEqual(x: GetContentTreeListQuery_Result, z: GetContentTreeListQuery_Result): boolean {
    return x.entityId === z.entityId;
  }
}

export interface ContentTreeListStoreCriteria {
  courseId: string;
}
