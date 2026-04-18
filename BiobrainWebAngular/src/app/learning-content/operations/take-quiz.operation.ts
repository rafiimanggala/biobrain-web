import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { Quiz } from '../../api/content/content-data-models';
import { AssignUserToIndividualQuizCommand } from '../../api/quiz-assignments/assign-user-to-individual-quiz.command';
import { EnsureQuizResultForAssignmentCommand } from '../../api/quiz-results/ensure-quiz-result-for-assignment.command';
import { GetLastIndividualUncompletedQuizResultQuery } from '../../api/quiz-results/get-last-individual-uncompleted-quiz-result.query';
import { GetQuizResultQuery } from '../../api/quiz-results/get-quiz-result.query';
import { CurrentUser } from '../../auth/services/current-user';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { RoutingService } from '../../auth/services/routing.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { AppSettings } from '../../share/values/app-settings';
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
    if (hasValue(getResult.quizResultId) && await this._hasResumableProgress(getResult.quizResultId, quiz)) {
      await this._routingService.navigateToQuizPage(getResult.quizResultId);
      return Result.success();
    }

    const assignResult = await this._api.send(new AssignUserToIndividualQuizCommand(user.userId, quiz.quizId)).toPromise();
    await this._routingService.navigateToQuizPage(assignResult.quizResultId);
    return Result.success();
  }

  /**
   * Guard against stale uncompleted assignments that already have all questions answered.
   *
   * Backend marks `CompletedAt` only when the last answer is submitted; if an earlier
   * attempt was abandoned after filling every slot (or if question count / excluded
   * questions shift after completion), the query can still return it. Without this
   * check the quiz page would see a "full" quiz result and redirect straight to the
   * result screen on the first REPEAT click.
   */
  private async _hasResumableProgress(quizResultId: string, quiz: Quiz): Promise<boolean> {
    try {
      const result = await this._api.send(new GetQuizResultQuery(quizResultId)).toPromise();
      const excludedQuestionIds = new Set(result.excludedQuestions ?? []);
      const eligibleQuestions = quiz.questions.filter(_ => !excludedQuestionIds.has(_.questionId)).length;
      const quizQuestionLimit = quiz.questionCount ?? AppSettings.quizQuestionsNumber;
      const maxQuestions = Math.min(eligibleQuestions, quizQuestionLimit);
      const answeredCount = result.questions?.length ?? 0;
      return answeredCount < maxQuestions;
    } catch {
      // If we cannot verify, fall back to safe default: start a fresh assignment.
      return false;
    }
  }
}
