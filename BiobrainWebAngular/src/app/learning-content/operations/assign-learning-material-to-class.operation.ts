import { Injectable } from '@angular/core';
import { Moment } from 'moment';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';

import { Api } from '../../api/api.service';
import {
  AssignLearningMaterialToClassCommand,
  AssignLearningMaterialToClassCommand_Result
} from '../../api/material-assignments/assign-learning-material-to-class.command';
import { GetTeacherSchoolClassesByCourseIdQuery } from '../../api/school-classes/get-teacher-school-classes-by-course-id.query';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ActiveCourseService } from '../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../core/services/active-school-class.service';
import { ActiveSchoolService } from '../../core/services/active-school.service';
import { ContentTreeService } from '../../core/services/content/content-tree.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { hasValue } from '../../share/helpers/has-value';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import {
  AssignLearningMaterialsAndQuizzesDialog
} from '../dialogs/assign-learning-materials-and-quizzes-dialog/assign-learning-materials-and-quizzes-dialog.component';
import {
  AssignLearningMaterialsAndQuizzesDialogData
} from '../dialogs/assign-learning-materials-and-quizzes-dialog/assign-learning-materials-and-quizzes-dialog.data';

@Injectable({
  providedIn: 'root',
})
export class AssignLearningMaterialToClassOperation {
  constructor(
    private readonly _dialog: Dialog,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _appEvents: AppEventProvider
  ) {
  }

  public async canPerform(_contentTreeNodeId: string): Promise<SuccessOrFailedResult<{
    schoolId: string;
    schoolClassIds: string[];
  }, string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      return Result.failed(this._strings.errors.noCurrentUser);
    }

    if (!user.isTeacher()) {
      return Result.failed(this._strings.errors.userIsNotTeacher);
    }

    const schoolId = await this._activeSchoolService.schoolId;
    if (!hasValue(schoolId)) {
      return Result.failed(this._strings.errors.userIsNotAssignedToSchool);
    }

    const schoolClassId = await this._activeSchoolClassService.schoolClassId;
    if (!hasValue(schoolClassId)) {
      return Result.failed(this._strings.errors.classIsNotSelected);
    }

    const courseId = await this._activeCourseService.courseId;
    if (!hasValue(courseId)) {
      return Result.failed(this._strings.errors.courseMustBeSelected);
    }

    const schoolClasses = await this._api.send(new GetTeacherSchoolClassesByCourseIdQuery(courseId, schoolId)).toPromise();
    const schoolClassIds = schoolClasses
      .map(x => x.schoolClassId)
      .filter(x => x !== schoolClassId);
    schoolClassIds.unshift(schoolClassId);

    return Result.success({ schoolId, schoolClassIds });
  }

  public async perform(contentTreeNodeId: string): Promise<SuccessOrFailedResult> {
    const canPerformResult = await this.canPerform(contentTreeNodeId);
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const { schoolId, schoolClassIds } = canPerformResult.data;

    const learningMaterialIds = [contentTreeNodeId];

    const dialogData = new AssignLearningMaterialsAndQuizzesDialogData(
      this._strings.assignLearningMaterial,
      learningMaterialIds,
      [],
      schoolId,
      schoolClassIds,
      [],
    );

    const dialogResult = await this._dialog.show(AssignLearningMaterialsAndQuizzesDialog, dialogData);
    if (!dialogResult.hasData()) {
      return Result.failed();
    }

    const result = await this._sendCommand(
      dialogResult.data.studentIdsBySchoolClassIdMap,
      learningMaterialIds,
      dialogResult.data.dueDate,
      false
    );

    if (result?.notAssignedNodeIds && result?.notAssignedNodeIds.length === 1) {
      const confirmation = await this._dialog.show(
        ConfirmationDialog,
        new ConfirmationDialogData(this._strings.quizAssignment, this._strings.messages.materialAssignmentConfirmation)
      );

      if (confirmation.action !== DialogAction.yes) {
        return Result.success();
      }

      await this._sendCommand(
        dialogResult.data.studentIdsBySchoolClassIdMap,
        result?.notAssignedNodeIds,
        dialogResult.data.dueDate,
        true
      );
    }

    return Result.success();
  }

  private async _sendCommand(
    studentIdsBySchoolClassIdMap: Record<string, string[]>,
    learningMaterialIds: string[],
    dueDateUtc: Moment,
    forceCreateNew: boolean
  ): Promise<AssignLearningMaterialToClassCommand_Result | null> {
    let result: AssignLearningMaterialToClassCommand_Result | null = null;
    try {
      result = await this._api.send(new AssignLearningMaterialToClassCommand(
        studentIdsBySchoolClassIdMap,
        learningMaterialIds,
        dueDateUtc,
        forceCreateNew
      )).toPromise();
    } catch (e: any) {
      if (e.errors?.Error[0]) {
        this._appEvents.errorEmit(e.errors.Error[0]);
      }
    }

    return result;
  }
}
