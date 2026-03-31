import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { EnsureQuizResultForAssignmentCommand } from '../../api/quiz-results/ensure-quiz-result-for-assignment.command';
import { GenerateSubsectionQuizCommand } from '../../api/quiz-results/generate-subsection-quiz.command';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { RoutingService } from '../../auth/services/routing.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';

@Injectable({
  providedIn: 'root',
})
export class TakeSubsectionQuizOperation {
  constructor(
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _routingService: RoutingService,
    private readonly _dialog: Dialog,
  ) {}

  public async canPerform(): Promise<SuccessOrFailedResult<{ userId: string }, string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      return Result.failed(this._strings.errors.noCurrentUser);
    }
    if (!user.isStudent()) {
      return Result.failed(this._strings.errors.userIsNotStudent);
    }

    return Result.success({ userId: user.userId });
  }

  public async perform(contentTreeNodeId: string): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform();
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const { userId } = canPerformResult.data;

    const generateResult = await firstValueFrom(
      this._api.send(new GenerateSubsectionQuizCommand(contentTreeNodeId, userId))
    );

    const ensureResult = await firstValueFrom(
      this._api.send(new EnsureQuizResultForAssignmentCommand(userId, generateResult.quizStudentAssignmentId))
    );

    await this._routingService.navigateToQuizPage(ensureResult.quizResultId);
    return Result.success();
  }
}
