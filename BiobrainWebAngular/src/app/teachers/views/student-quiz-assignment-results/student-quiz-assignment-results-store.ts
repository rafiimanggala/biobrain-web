import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';
import { GetStudentQuizAssignmentResultsQuery } from 'src/app/api/quizzes/get-student-quiz-assignment-results.query';
import { BindableItemStore } from 'src/app/core/stores/bindable-item-store';

import { Api } from '../../../api/api.service';
import {
  QuizAnalyticOutput_CourseQuizInfo,
  QuizAnalyticOutput_Student,
  QuizAnalyticOutput_SubjectInfo,
} from '../../../api/quizzes/quiz-analytic-output.models';
import { ContentTreeService } from '../../../core/services/content/content-tree.service';

@Injectable()
export class StudentQuizAssignmentResultsStore extends BindableItemStore<StudentQuizAssignmentResultsStoreItem, Criteria> {
  constructor(private readonly _api: Api, private readonly _contentTreeService: ContentTreeService) {
    super();
  }

  protected getItem(criteria: Criteria): Observable<StudentQuizAssignmentResultsStoreItem> {
    const query = new GetStudentQuizAssignmentResultsQuery(criteria.studentId, criteria.schoolClassId, criteria.courseId);
    return this._api.send(query).pipe(
      switchMap(
        result => this._contentTreeService.providerForGetNodeById().pipe(
          map(getNodeById => ({
            ...result,
            results: result.results.map(r => ({
              ...r,
              quizName: getNodeById(r.contentTreeNodeId).fullName,
            })),
          })),
        ),
      ),
    );
  }
}

export interface Criteria {
  studentId: string;
  schoolClassId: string;
  courseId: string;
}

export interface StudentQuizAssignmentResultsStoreItem {
  readonly studentInfo: QuizAnalyticOutput_Student;
  readonly classData: QuizAnalyticOutput_CourseQuizInfo;
  readonly subjectData: QuizAnalyticOutput_SubjectInfo;
  readonly results: StudentQuizAssignmentResultsStoreItem_Result_Item[];
}

export interface StudentQuizAssignmentResultsStoreItem_Result_Item {
  readonly quizStudentAssignmentId: string;
  readonly quizAssignmentId: string;
  readonly quizId: string;
  readonly contentTreeNodeId: string;
  readonly score: number;
  readonly progress: number;
  readonly completedAt: string | null | undefined;
  readonly notApplicable: boolean;
  readonly quizNameHtml: string;
  readonly quizName: string;
}
