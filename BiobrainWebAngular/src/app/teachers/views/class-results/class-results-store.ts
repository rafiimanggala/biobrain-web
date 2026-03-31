import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import {
  ClassResultsQuery_Result_QuizResultData,
  GetClassResultsQuery,
} from 'src/app/api/quizzes/get-class-results.query';
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
export class ClassResultsStore extends BindableItemStore<ClassResultsStoreItem, Criteria> {
  constructor(private readonly _api: Api, private readonly _contentTreeService: ContentTreeService) {
    super();
  }

  protected getItem(criteria: Criteria): Observable<ClassResultsStoreItem> {
    // console.log(criteria);
    const query = new GetClassResultsQuery(criteria.schoolClassId, criteria.courseId, criteria.selectedFilterNodes);

    return this._api.send(query).pipe(
      switchMap(
        result => this._contentTreeService.providerForGetNodeById().pipe(
          map(getNodeById => ({
            ...result,
            quizAssignments: result.quizAssignments.map(r => ({
              ...r,
              quizName: r.quizName,
              
            })),
          })),
        ),
      ),
    );
  }
}

export interface Criteria {
  readonly schoolClassId: string;
  readonly courseId: string;
  readonly selectedFilterNodes: string[];
}


export interface ClassResultsStoreItem {
  readonly classData: QuizAnalyticOutput_CourseQuizInfo;
  readonly subjectData: QuizAnalyticOutput_SubjectInfo;
  readonly quizAssignments: ClassResultsStoreItem_QuizAssignment[];
  readonly quizStudentAssignments: ClassResultsStoreItem_QuizStudentAssignment[];
  readonly students: QuizAnalyticOutput_Student[];
  readonly averageScoreInfo: QuizAnalyticOutput_AverageScoreData[];
  readonly progressInfo: QuizAnalyticOutput_ProgressData[];
  readonly quizResults: ClassResultsQuery_Result_QuizResultData[];
}

export interface ClassResultsStoreItem_QuizAssignment {
  readonly assignedByTeacherId: string;
  readonly contentTreeNodeId: string;
  readonly quizId: string;
  readonly quizAssignmentId: string;
  readonly quizName: string;
  readonly path: string[];
}

export interface ClassResultsStoreItem_QuizStudentAssignment {
  readonly quizStudentAssignmentId: string;
  readonly quizAssignmentId: string;
  readonly assignedToUserId: string;
}
