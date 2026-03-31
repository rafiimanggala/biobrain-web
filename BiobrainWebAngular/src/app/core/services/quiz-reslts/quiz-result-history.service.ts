import { Injectable } from '@angular/core';
import moment from 'moment';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { GetQuizResultHistoryQuery } from 'src/app/api/quiz-results/get-quiz-result-history.query';

import { Api } from '../../../api/api.service';

import { HistoryQuizResult } from './history-quiz-result';
import { QuizResultHistory } from './quiz-result-history';

@Injectable({
  providedIn: 'root',
})
export class QuizResultHistoryService {
  // private readonly _cacheById = new ObservableCache<QuizResult>();

  constructor(private readonly _api: Api) {
  }

  observeQuizResult(courseId: string): Observable<QuizResultHistory> {
    return this._getQuizResultHistory(courseId); //this._cacheById.get(quizResultId, () => this._getQuizResult(quizResultId));
  }

  // async saveAnswerValue(quizResultId: string, questionId: string, value: string, isCorrect: boolean): Promise<void> {
  //   await this._api.send(new SetQuizResultQuestionValueCommand(quizResultId, questionId, value, isCorrect)).toPromise();
  //   this._cacheById.reload(quizResultId);
  // }

  private _getQuizResultHistory(courseId: string): Observable<QuizResultHistory> {
    return this._api.send(new GetQuizResultHistoryQuery(courseId)).pipe(
      map(_ => new QuizResultHistory(
        _.averageQuizRate,
        _.quizzesCompletedRate,
        _.quizResults.map(q => new HistoryQuizResult(q.quizResultId, q.quizId, q.courseId, q.unitId, q.nodeId, q.parentNodeId, q.path, q.nameLines, q.score, moment(q.date).format("DD MMM YY"))),
      )),
    );
  }
}
