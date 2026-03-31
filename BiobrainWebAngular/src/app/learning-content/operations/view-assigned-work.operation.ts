import { Injectable } from '@angular/core';
import { ActiveSchoolService } from 'src/app/core/services/active-school.service';

import { Api } from '../../api/api.service';
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
export class ViewAssignedWorkOperation {
  constructor(
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _routingService: RoutingService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _dialog: Dialog,
  ) {
  }

  public async canPerform(nodeId: string): Promise<SuccessOrFailedResult<{ unitNode: ContentTreeNode; user: CurrentUser }, string>> {
    const node = await firstValueFrom(this._contentTreeService.getNode(nodeId));
    if (!hasValue(node)) return Result.failed(this._strings.errors.nodeWasNotFound);

    const user = await this._currentUserService.user;
    if (!hasValue(user)) return Result.failed(this._strings.errors.noCurrentUser);
    if (!user.isStudent()) return Result.failed(this._strings.errors.userIsNotStudent);
    var schoolId = await this._activeSchoolService.schoolId;
    if(!hasValue(schoolId) || schoolId.length < 1) return Result.failed(this._strings.errors.userIsNotAssignedToSchool);

    return Result.success({ unitNode: node.root, user });
  }

  public async perform(nodeId: string): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform(nodeId);
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const { unitNode } = canPerformResult.data;
    await this._routingService.navigateToAssignedWork(unitNode.nodeId);
    return Result.success();
  }
}
