import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { Api } from '../../../api/api.service';
import { GetQuizResultQuery } from '../../../api/quiz-results/get-quiz-result.query';
import { SetQuizResultQuestionValueCommand } from '../../../api/quiz-results/set-quiz-result-question-value-command';
import { ObservableCache } from '../../../share/helpers/observable-cache';
import { LoadingProcess, loadingProcessOf } from '../../../share/helpers/observable-operators';

import { QuizResult } from './quiz-result';
import { QuizResultQuestion } from './quiz-result-question';

@Injectable({
  providedIn: 'root',
})
export class QuizResultService {
  private readonly _cacheById = new ObservableCache<LoadingProcess<QuizResult>>();

  constructor(private readonly _api: Api) {
  }

  observeQuizResult(quizResultId: string): Observable<QuizResult> {
    return this.observeQuizLoadingProcess(quizResultId).pipe(switchMap(_ => _.data$));
  }

  observeQuizLoadingProcess(quizResultId: string): Observable<LoadingProcess<QuizResult>> {
    return this._cacheById.get(quizResultId, () => of(loadingProcessOf(this._getQuizResult(quizResultId))));
  }

  async saveAnswerValue(quizResultId: string, questionId: string, value: string, isCorrect: boolean): Promise<void> {
    await this._api.send(new SetQuizResultQuestionValueCommand(quizResultId, questionId, new Date(), value, isCorrect)).toPromise();
    this._cacheById.reload(quizResultId);
  }

  private _getQuizResult(quizResultId: string): Observable<QuizResult> {
    return this._api.send(new GetQuizResultQuery(quizResultId)).pipe(
      map(_ => new QuizResult(
        _.quizResultId,
        _.quizId,
        _.userId,
        _.schoolClassId,
        _.schoolId,
        _.schoolName,
        _.score,
        _.questions.map(q => new QuizResultQuestion(q.quizResultId, q.questionId, q.value, q.isCorrect)),
        _.excludedQuestions,
        _.hintsEnabled !== false,
        _.soundEnabled !== false
      )),
    );
  }
}
