import { Injectable } from '@angular/core';
import { forkJoin, from, Observable, of } from 'rxjs';
import { catchError, map, shareReplay, switchMap } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import { GetQuizByIdQuery } from '../../../api/content/get-quiz-by-id.query';
import { LearningContentProviderService } from '../../../learning-content/services/learning-content-provider.service';
import { QuizRow, learningContentDb } from '../../../learning-content/services/learning-content-db';
import { hasValue } from '../../../share/helpers/has-value';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { toDictionary } from '../../../share/helpers/observable-operators';
import { ContentTreeService } from '../content/content-tree.service';

import { Quiz } from './quiz';

@Injectable({
  providedIn: 'root',
})
export class QuizzesService {
  private _items$!: Observable<Quiz[]>;
  private _indexById$!: Observable<Map<string, Quiz>>;
  private _indexByNodeId$!: Observable<Map<string, Quiz>>;

  constructor(
    private readonly _learningContentProviderService: LearningContentProviderService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _api: Api,
  ) {
    this._rebuildObservables();
  }

  private _rebuildObservables(): void {
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

  public async reloadAndWait(): Promise<void> {
    this._rebuildObservables();
    await firstValueFrom(this._items$);
  }

  public getById(quizId: string): Observable<Quiz> {
    return this._indexById$.pipe(
      switchMap(index => {
        const cached = index.get(quizId);
        if (cached) {
          return of(cached);
        }
        return this._fetchAndCacheQuiz(quizId);
      }),
      shareReplay(1),
    );
  }

  private _fetchAndCacheQuiz(quizId: string): Observable<Quiz> {
    return this._api.send(new GetQuizByIdQuery(quizId)).pipe(
      switchMap(result => {
        if (!result) {
          throw new Error('Quiz was not found');
        }
        const quizRow = new QuizRow(
          result.quizId,
          result.courseId,
          result.contentTreeNodeId,
          result.questions,
        );
        return from(learningContentDb.quizzes.put(quizRow)).pipe(
          switchMap(() => this._contentTreeService.getNode(result.contentTreeNodeId).pipe(
            map(contentTreeNode => new Quiz(quizRow, contentTreeNode)),
          )),
        );
      }),
    );
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

