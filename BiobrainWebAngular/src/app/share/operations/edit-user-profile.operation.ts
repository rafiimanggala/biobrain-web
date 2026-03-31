import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { GetStudentByIdQuery } from '../../api/students/get-student-by-id.query';
import { UpdateStudentCommand } from '../../api/students/update-student.command';
import { GetTeacherByIdQuery } from '../../api/teachers/get-teacher-by-id.query';
import { UpdateTeacherDetailsCommand } from '../../api/teachers/update-teacher-details.command';
import { RefreshAuthorizationOperation } from '../../auth/operations/refresh-authorization.operation';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { DialogAction } from '../../core/dialogs/dialog-action';
import { Dialog } from '../../core/dialogs/dialog.service';
import { UserProfileDialogData } from '../dialogs/user-profile-dialog/user-profile-dialog-data';
import { UserProfileDialogComponent } from '../dialogs/user-profile-dialog/user-profile-dialog.component';
import { firstValueFrom } from '../helpers/first-value-from';
import { hasValue } from '../helpers/has-value';
import { Result, SuccessOrFailedResult } from '../helpers/result';
import { LoaderService } from '../services/loader.service';
import { SnackBarService } from '../services/snack-bar.service';
import { StringsService } from '../strings.service';
import { DeleteConfirmationDialog } from '../dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from '../dialogs/delete-confirmation/delete-confirmation.dialog-data';

@Injectable({
  providedIn: 'root',
})
export class EditUserProfileOperation {
  constructor(
    private readonly _strings: StringsService,
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _snackBarService: SnackBarService,
    private readonly _refreshAuthorizationOperation: RefreshAuthorizationOperation
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult<{userId: string; userType?: 'student' | 'teacher'}, string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) return Result.failed(this._strings.errors.noCurrentUser);

    const studentType = user.isStudent() ? 'student' : undefined;
    const teacherType = user.isTeacher() ? 'teacher' : undefined;
    return Result.success({ userId: user.userId, userType: studentType ?? teacherType});
  }

  async perform(): Promise<SuccessOrFailedResult> {
    const canPerform = await this.canPerform();
    if (canPerform.isFailed()) {
      this._snackBarService.showMessage(canPerform.reason);
      return Result.failed();
    }

    const { userId, userType } = canPerform.data;

    if (userType === 'student') return this._editStudentProfile(userId);
    if (userType === 'teacher') return this._editTeacherProfile(userId);
    return Result.failed();
  }

  private async _editStudentProfile(userId: string): Promise<SuccessOrFailedResult> {
    const student = await firstValueFrom(this._api.send(new GetStudentByIdQuery(userId)));
    const dialogData = new UserProfileDialogData(student.firstName, student.lastName, student.email, this._strings.student, true, student.country, student.state, student.curriculumCode);
    const dialogResult = await this._dialog.show(UserProfileDialogComponent, dialogData);

    if (dialogResult.action !== DialogAction.save || !dialogResult.hasData()) return Result.failed();

    this._loaderService.show();
    try {
      const cmd = new UpdateStudentCommand(userId, dialogResult.data.firstName, dialogResult.data.lastName, dialogResult.data.country, dialogResult.data.state, dialogResult.data.curriculumCode);
      await firstValueFrom(this._api.send(cmd));
      await this._refreshAuthorizationOperation.perform();
      return Result.success();
    } catch (e) {
      return Result.failed();
    } finally {
      this._loaderService.hide();
    }
  }

  private async _editTeacherProfile(userId: string): Promise<SuccessOrFailedResult> {
    const teacher = await firstValueFrom(this._api.send(new GetTeacherByIdQuery(userId)));
    const dialogData = new UserProfileDialogData(teacher.firstName, teacher.lastName, teacher.email, this._strings.teacher, false);
    const dialogResult = await this._dialog.show(UserProfileDialogComponent, dialogData);    

    if (dialogResult.action !== DialogAction.save || !dialogResult.hasData()) return Result.failed();

    this._loaderService.show();
    try {
      const cmd = new UpdateTeacherDetailsCommand(userId, dialogResult.data.firstName, dialogResult.data.lastName);
      await firstValueFrom(this._api.send(cmd));
      await this._refreshAuthorizationOperation.perform();
      return Result.success();
    } catch (e) {
      return Result.failed();
    } finally {
      this._loaderService.hide();
    }
  }
}
