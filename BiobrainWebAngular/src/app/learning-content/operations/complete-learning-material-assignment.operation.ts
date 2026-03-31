import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { SetAssignedLearningMaterialAsDoneCommand } from '../../api/material-assignments/set-assigned-learning-material-as-done.command';
import { CurrentUser } from '../../auth/services/current-user';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { RoutingService } from '../../auth/services/routing.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ContentTreeNode } from '../../core/services/content/content-tree.node';
import { ContentTreeService } from '../../core/services/content/content-tree.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';

@Injectable({
  providedIn: 'root',
})
export class CompleteLearningMaterialAssignmentOperation {
  constructor(
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _routingService: RoutingService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _dialog: Dialog,
  ) {
  }

  public async canPerform(nodeId: string): Promise<SuccessOrFailedResult<{ courseId: string; topic: ContentTreeNode; user: CurrentUser }, string>> {
    const node = await firstValueFrom(this._contentTreeService.getNode(nodeId));
    if (!hasValue(node)) {
      return Result.failed(this._strings.errors.nodeWasNotFound);
    }

    if (!hasValue(node.parent)) {
      return Result.failed(this._strings.errors.nodeHasNoParent);
    }

    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      return Result.failed(this._strings.errors.noCurrentUser);
    }

    if (!user.isStudent()) {
      return Result.failed(this._strings.errors.userIsNotStudent);
    }

    return Result.success({ courseId: node.row.courseId, topic: node.parent, user });
  }

  public async perform(assignmentId: string, nodeId: string): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform(nodeId);
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const { courseId, topic } = canPerformResult.data;

    const completeResult = await this._api.send(new SetAssignedLearningMaterialAsDoneCommand(assignmentId)).toPromise();
    if (completeResult && completeResult.completedAtUtc) {
      await this._routingService.navigateToMaterialPage(courseId, topic.nodeId, undefined);
      return Result.success();
    }

    return Result.failed();
  }
}
