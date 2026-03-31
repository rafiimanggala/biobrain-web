import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { FailedOrSuccessResult, Result } from '../../share/helpers/result';
import { GetQuizResultForLevelQuery, GetQuizResultForLevelQuery_ResultQuizResults } from 'src/app/api/quiz-results/get-quiz-result-for-level.query';

@Injectable({
  providedIn: 'root',
})
export class GetQuizResultForLevelOperation {
  constructor(
    private readonly _api: Api,
  ) {
  }

  public async perform(courseId: string, levelIds: string[]): Promise<FailedOrSuccessResult<any, GetQuizResultForLevelQuery_ResultQuizResults[]>> {
    try{
      const results = await this._api.send(new GetQuizResultForLevelQuery(courseId, levelIds)).toPromise();
      return Result.success(results.quizResults);
    }
    catch(e: any){
      return Result.failed(e);
    }
  }
}
