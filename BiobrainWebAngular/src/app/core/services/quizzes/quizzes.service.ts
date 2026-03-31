import { Injectable } from '@angular/core';
import { forkJoin, from, Observable, of, NEVER } from 'rxjs';
import { catchError, filter, map, shareReplay, switchMap, tap } from 'rxjs/operators';

import { LearningContentProviderService } from '../../../learning-content/services/learning-content-provider.service';
import { hasValue } from '../../../share/helpers/has-value';
import { toDictionary } from '../../../share/helpers/observable-operators';
import { toNonNullableWithError } from '../../../share/helpers/to-non-nullable';
import { ContentTreeService } from '../content/content-tree.service';

import { Quiz } from './quiz';

@Injectable({
  providedIn: 'root',
})
export class QuizzesService {
  private readonly _items$: Observable<Quiz[]>;
  private readonly _indexById$: Observable<Map<string, Quiz>>;
  private readonly _indexByNodeId$: Observable<Map<string, Quiz>>;

  constructor(
    private readonly _learningContentProviderService: LearningContentProviderService,
    private readonly _contentTreeService: ContentTreeService,
  ) {
    this._items$ = from(this._learningContentProviderService.getAllQuizRows()).pipe(
      switchMap(
        quizzes => forkJoin(quizzes.map(
          quizRow => this._contentTreeService.getNode(quizRow.contentTreeNodeId).pipe(
            map(contentTreeNode => new Quiz(quizRow, contentTreeNode)),
            catchError(() => of(undefined)),      
          ),
        )),
      ),
      map(items => items.filter(hasValue)),
      shareReplay(1),
    );

    this._indexById$ = this._items$.pipe(toDictionary(_ => _.row.quizId));
    this._indexByNodeId$ = this._items$.pipe(toDictionary(_ => _.row.contentTreeNodeId));
    
  }

  public getById(quizId: string): Observable<Quiz> {
    // console.log(this._indexById$);
    return this._indexById$.pipe(map(index => index.get(quizId)), map(toNonNullableWithError('Quiz was not found')), shareReplay(1));
  }

  public getByIds(quizIds: string[]): Observable<Quiz[]> {
    return forkJoin(quizIds.map(quizId => this.getById(quizId)));
  }

  public findByNodeId(nodeId: string): Observable<Quiz | undefined> {
    return this._indexByNodeId$.pipe(map(index => index.get(nodeId)), shareReplay(1));
  }

  public getByNodeIds(nodeIds: string[]): Observable<Quiz[]> {
    return forkJoin(nodeIds.map(quizId => this.findByNodeId(quizId))).pipe(map(_ => _.filter(hasValue)));
  }
}

