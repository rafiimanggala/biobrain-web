import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { Quiz } from '../../api/content/content-data-models';
import { AssignUserToIndividualQuizCommand } from '../../api/quiz-assignments/assign-user-to-individual-quiz.command';
import { EnsureQuizResultForAssignmentCommand } from '../../api/quiz-results/ensure-quiz-result-for-assignment.command';
import { GetLastIndividualUncompletedQuizResultQuery } from '../../api/quiz-results/get-last-individual-uncompleted-quiz-result.query';
import { CurrentUser } from '../../auth/services/current-user';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { RoutingService } from '../../auth/services/routing.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { LearningContentProviderService } from '../services/learning-content-provider.service';

@Injectable({
  providedIn: 'root',
})
export class TakeQuizOperation {

  constructor(
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _learningContentProviderService: LearningContentProviderService,
    private readonly _routingService: RoutingService,
    private readonly _dialog: Dialog,
  ) {
  }

  public async canPerform(courseId: string, contentTreeNodeId: string): Promise<SuccessOrFailedResult<{quiz: Quiz; user: CurrentUser}, string>> {
    const quiz = await this._learningContentProviderService.getQuizzByNodeId(courseId, contentTreeNodeId);
    if (!hasValue(quiz)) {
      return Result.failed(this._strings.errors.quizWasNotFound);
    }

    if (quiz.questions.length === 0) {
      return Result.failed(this._strings.errors.quizHasNoQuestion);
    }

    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      return Result.failed(this._strings.errors.noCurrentUser);
    }
    if (!user.isStudent()) {
      return Result.failed(this._strings.errors.userIsNotStudent);
    }

    return Result.success({ quiz, user });
  }

  public async performAssignedQuiz(courseId: string, contentTreeNodeId: string, quizStudentAssignmentId: string): Promise<FailedOrSuccessResult> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: this._strings.errors.noCurrentUser });
      return Result.failed();
    }
    if (!user.isStudent()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: this._strings.errors.userIsNotStudent });
      return Result.failed();
    }

    try {
      const ensureResult = await firstValueFrom(this._api.send(new EnsureQuizResultForAssignmentCommand(user.userId, quizStudentAssignmentId)));
      await this._routingService.navigateToQuizPage(ensureResult.quizResultId);
      return Result.success();
    } catch (err) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: this._strings.errors.quizWasNotFound });
      return Result.failed();
    }
  }

  public async perform(courseId: string, contentTreeNodeId: string): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform(courseId, contentTreeNodeId);
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const { quiz, user } = canPerformResult.data;

    const getResult = await this._api.send(new GetLastIndividualUncompletedQuizResultQuery(user.userId, quiz.quizId)).toPromise();
    if (hasValue(getResult.quizResultId)) {
      await this._routingService.navigateToQuizPage(getResult.quizResultId);
      return Result.success();
    }

    const assignResult = await this._api.send(new AssignUserToIndividualQuizCommand(user.userId, quiz.quizId)).toPromise();
    await this._routingService.navigateToQuizPage(assignResult.quizResultId);
    return Result.success();
  }
}
