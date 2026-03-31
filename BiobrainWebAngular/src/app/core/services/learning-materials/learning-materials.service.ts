import { Injectable } from '@angular/core';
import { forkJoin, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ContentTreeService } from '../content/content-tree.service';

import { LearningMaterial } from './learning.material';

@Injectable({
  providedIn: 'root',
})
export class LearningMaterialsService {

  constructor(
    private readonly _contentTreeService: ContentTreeService,
  ) {
  }

  public getById(nodeId: string): Observable<LearningMaterial> {
    return this._contentTreeService.getNode(nodeId).pipe(
      map(contentTreeNode => new LearningMaterial(contentTreeNode.row, contentTreeNode)),
    );
  }

  public getByIds(nodeIds: string[]): Observable<LearningMaterial[]> {
    return forkJoin(nodeIds.map(quizId => this.getById(quizId)));
  }
}
