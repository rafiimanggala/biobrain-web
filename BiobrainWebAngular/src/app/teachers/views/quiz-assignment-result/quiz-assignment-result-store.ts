import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import {
  GetQuizAssignmentResultQuery,
  GetQuizAssignmentResultQuery_Result_Question,
  GetQuizAssignmentResultQuery_Result_QuestionResult,
} from 'src/app/api/quizzes/get-quiz-assignment-result.query';
import { BindableItemStore } from 'src/app/core/stores/bindable-item-store';

import { Api } from '../../../api/api.service';
import {
  QuizAnalyticOutput_AverageScoreData,
  QuizAnalyticOutput_CourseQuizInfo,
  QuizAnalyticOutput_ProgressData,
  QuizAnalyticOutput_Student,
  QuizAnalyticOutput_SubjectInfo,
} from '../../../api/quizzes/quiz-analytic-output.models';
import { ContentTreeService } from '../../../core/services/content/content-tree.service';

@Injectable()
export class QuizAssignmentResultStore extends BindableItemStore<QuizAssignmentResultStoreItem, Criteria> {
  constructor(private readonly _api: Api, private readonly _contentTreeService: ContentTreeService) {
    super();
  }

  protected getItem(criteria: Criteria): Observable<QuizAssignmentResultStoreItem> {
    const query = new GetQuizAssignmentResultQuery(criteria.quizAssignmentId);

    return this._api.send(query).pipe(
      switchMap(result => this._contentTreeService.providerForGetNodeById().pipe(
        map(getNodeById => ({
          ...result,
          // quizName: getNodeById(result.contentTreeNodeId).fullName,
        })),
      )),
    );
  }
}

export interface Criteria {
  readonly quizAssignmentId: string;
}

export interface QuizAssignmentResultStoreItem {
  readonly contentTreeNodeId: string;
  readonly quizId: string;
  readonly classData: QuizAnalyticOutput_CourseQuizInfo | undefined | null;
  readonly subjectData: QuizAnalyticOutput_SubjectInfo;
  readonly students: QuizAnalyticOutput_Student[];
  readonly averageScoreInfo: QuizAnalyticOutput_AverageScoreData[];
  readonly progressInfo: QuizAnalyticOutput_ProgressData[];
  readonly questions: GetQuizAssignmentResultQuery_Result_Question[];
  readonly questionResults: GetQuizAssignmentResultQuery_Result_QuestionResult[];
  readonly quizName: string;
}
