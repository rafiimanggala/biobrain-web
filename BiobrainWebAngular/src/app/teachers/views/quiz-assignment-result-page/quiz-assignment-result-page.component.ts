import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { filter, pluck } from 'rxjs/operators';

import { hasValue } from '../../../share/helpers/has-value';
import { ReassignQuizzesToStudentsOperation } from '../../../learning-content/operations/reassign-quizzes-to-students.operation';

@Component({
  selector: 'app-quiz-assignment-result-page',
  templateUrl: './quiz-assignment-result-page.component.html',
  styleUrls: ['./quiz-assignment-result-page.component.scss'],
})
export class QuizAssignmentResultPageComponent {
  readonly quizAssignmentIdChanges$: Observable<string>;

  constructor(
    private readonly _route: ActivatedRoute,
    private readonly _reassignQuizzesToStudentsOperation: ReassignQuizzesToStudentsOperation,
  ) {
    this.quizAssignmentIdChanges$ = this._route.params.pipe(
      pluck('quizAssignmentId'),
      filter(hasValue),
    );
  }

  async onReassignQuiz(quizAssignmentId: string, quizId: string, studentIds: string[]): Promise<void> {
    await this._reassignQuizzesToStudentsOperation.perform(studentIds, [quizAssignmentId], [], [quizId]);
  }
}
