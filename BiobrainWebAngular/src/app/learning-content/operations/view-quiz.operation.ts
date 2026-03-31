import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { Quiz } from '../../api/content/content-data-models';
import { CurrentUser } from '../../auth/services/current-user';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { RoutingService } from '../../auth/services/routing.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { LearningContentProviderService } from '../services/learning-content-provider.service';

@Injectable({
  providedIn: 'root',
})
export class ViewQuizOperation {

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
    if (!user.isTeacher()) {
      return Result.failed(this._strings.errors.userIsNotStudent);
    }

    return Result.success({ quiz, user });
  }

  public async perform(courseId: string, contentTreeNodeId: string): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform(courseId, contentTreeNodeId);
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const { quiz, user } = canPerformResult.data;

    await this._routingService.navigateToViewQuizPage(quiz.quizId);
    return Result.success();
  }
}
