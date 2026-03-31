import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { EnsureQuizResultForAssignmentCommand } from '../../api/quiz-results/ensure-quiz-result-for-assignment.command';
import { GenerateTopicQuizCommand } from '../../api/quiz-results/generate-topic-quiz.command';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { RoutingService } from '../../auth/services/routing.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { TopicQuizDialogComponent } from '../dialogs/topic-quiz-dialog/topic-quiz-dialog.component';
import { TopicQuizDialogData } from '../dialogs/topic-quiz-dialog/topic-quiz-dialog-data';
import { TopicQuizDialogResult } from '../dialogs/topic-quiz-dialog/topic-quiz-dialog-result';
import { ContentTreeService } from '../../core/services/content/content-tree.service';

@Injectable({
  providedIn: 'root',
})
export class TakeTopicQuizOperation {
  constructor(
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _routingService: RoutingService,
    private readonly _dialog: Dialog,
    private readonly _contentTreeService: ContentTreeService,
  ) {}

  public async canPerform(courseId: string, contentTreeNodeId: string): Promise<SuccessOrFailedResult<{ userId: string }, string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      return Result.failed(this._strings.errors.noCurrentUser);
    }
    if (!user.isStudent()) {
      return Result.failed(this._strings.errors.userIsNotStudent);
    }

    return Result.success({ userId: user.userId });
  }

  public async perform(courseId: string, contentTreeNodeId: string): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform(courseId, contentTreeNodeId);
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const node = await firstValueFrom(this._contentTreeService.findNode(contentTreeNodeId));
    const topicName = node?.row.name ?? '';

    const dialogData = new TopicQuizDialogData(topicName, contentTreeNodeId, courseId);
    const dialogResult = await this._dialog.show<TopicQuizDialogData, TopicQuizDialogResult>(
      TopicQuizDialogComponent,
      dialogData,
      { width: '400px' },
    );

    if (!dialogResult || dialogResult.action !== DialogAction.save || !dialogResult.data) {
      return Result.failed();
    }

    const { questionCount } = dialogResult.data;
    const { userId } = canPerformResult.data;

    const generateResult = await firstValueFrom(
      this._api.send(new GenerateTopicQuizCommand(contentTreeNodeId, questionCount, userId))
    );

    const ensureResult = await firstValueFrom(
      this._api.send(new EnsureQuizResultForAssignmentCommand(userId, generateResult.quizStudentAssignmentId))
    );

    await this._routingService.navigateToQuizPage(ensureResult.quizResultId);
    return Result.success();
  }
}
