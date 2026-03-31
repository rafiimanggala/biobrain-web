import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { filter, pluck } from 'rxjs/operators';

import { ActiveCourseService } from '../../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../../core/services/active-school-class.service';
import { hasValue } from '../../../share/helpers/has-value';
import { ReassignQuizzesToStudentsOperation } from '../../../learning-content/operations/reassign-quizzes-to-students.operation';
import { ReassignData } from '../student-quiz-assignment-results/student-quiz-assignment-results.component';

@Component({
  selector: 'app-student-quiz-assignment-results-page',
  templateUrl: './student-quiz-assignment-results-page.component.html',
  styleUrls: ['./student-quiz-assignment-results-page.component.scss'],
})
export class StudentQuizAssignmentResultsPageComponent {
  readonly studentIdChanges$: Observable<string>;
  readonly courseIdChanges$: Observable<string>;
  readonly schoolClassIdChanges$: Observable<string>;

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _reassignQuizzesToStudentsOperation: ReassignQuizzesToStudentsOperation
  ) {
    this.studentIdChanges$ = this._route.params.pipe(
      pluck('studentId'),
      filter(hasValue),
    );

    this.courseIdChanges$ = this._activeCourseService.courseIdChanges$.pipe(filter(hasValue));
    this.schoolClassIdChanges$ = this._activeSchoolClassService.schoolClassIdChanges$.pipe(filter(hasValue));
  }

  async onReassign(reassignData: ReassignData, studentId: string): Promise<void> {
    await this._reassignQuizzesToStudentsOperation.perform([studentId], reassignData.quizAssignmentIds, reassignData.materialIds, reassignData.quizIds);
  }
}
