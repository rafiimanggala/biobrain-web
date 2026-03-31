import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { GetQuizResultStreakCommand } from 'src/app/api/quiz-results/get-quiz-result-streak.query';

import { Api } from '../../../api/api.service';

import { QuizResultStreak } from './quiz-result-streak';

@Injectable({
  providedIn: 'root',
})
export class QuizResultStreakService {
  // private readonly _cacheById = new ObservableCache<QuizResult>();

  constructor(private readonly _api: Api) {
  }

  observeQuizResult(courseId: string): Observable<QuizResultStreak> {
    return this._getQuizResultStreak(courseId); //this._cacheById.get(quizResultId, () => this._getQuizResult(quizResultId));
  }

  // async saveAnswerValue(quizResultId: string, questionId: string, value: string, isCorrect: boolean): Promise<void> {
  //   await this._api.send(new SetQuizResultQuestionValueCommand(quizResultId, questionId, value, isCorrect)).toPromise();
  //   this._cacheById.reload(quizResultId);
  // }

  private _getQuizResultStreak(courseId: string): Observable<QuizResultStreak> {
    return this._api.send(new GetQuizResultStreakCommand(courseId, new Date())).pipe(
      map(_ => new QuizResultStreak(
        _.streak,
        _.daysCount,
      )),
    );
  }
}
