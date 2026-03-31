import { Injectable } from '@angular/core';
import { Moment } from 'moment';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';

import { Api } from '../../api/api.service';
import {
  AssignQuizzesToClassCommand,
  AssignQuizzesToClassCommand_Result
} from '../../api/quiz-assignments/assign-quizzes-to-class.command';
import { GetTeacherSchoolClassesByCourseIdQuery } from '../../api/school-classes/get-teacher-school-classes-by-course-id.query';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ActiveCourseService } from '../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../core/services/active-school-class.service';
import { ActiveSchoolService } from '../../core/services/active-school.service';
import { Quiz } from '../../core/services/quizzes/quiz';
import { QuizzesService } from '../../core/services/quizzes/quizzes.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from '../../share/helpers/result';
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
export class AssignQuizToClassOperation {
  constructor(
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _dialog: Dialog,
    private readonly _quizzesService: QuizzesService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _appEvents: AppEventProvider,
    private readonly _activeCourseService: ActiveCourseService,
  ) {
  }

  public async canPerform(contentTreeNodeId: string): Promise<
  SuccessOrFailedResult<{ quiz: Quiz; schoolId: string; schoolClassIds: string[] },
  string>> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) {
      return Result.failed(this._strings.errors.noCurrentUser);
    }
    if (!user.isTeacher()) {
      return Result.failed(this._strings.errors.userIsNotTeacher);
    }

    const quiz = await this._quizzesService.findByNodeId(contentTreeNodeId).toPromise();
    if (!hasValue(quiz)) {
      return Result.failed(this._strings.errors.quizIsNotAssignedToThisMaterial);
    }
    if (quiz.row.questions.length === 0) {
      return Result.failed(this._strings.errors.quizHasNoQuestion);
    }

    const schoolId = await this._activeSchoolService.schoolId;
    if (!hasValue(schoolId)) {
      return Result.failed(this._strings.errors.userIsNotAssignedToSchool);
    }

    const courseId = await this._activeCourseService.courseId;
    if (!hasValue(courseId)) {
      return Result.failed(this._strings.errors.courseMustBeSelected);
    }

    const schoolClassId = await this._activeSchoolClassService.schoolClassId;
    if (!hasValue(schoolClassId)) {
      return Result.failed(this._strings.errors.classIsNotSelected);
    }

    const schoolClasses = await this._api.send(new GetTeacherSchoolClassesByCourseIdQuery(courseId, schoolId)).toPromise();
    const schoolClassIds = schoolClasses
      .map(x => x.schoolClassId)
      .filter(x => x !== schoolClassId);
    schoolClassIds.unshift(schoolClassId);

    return Result.success({ quiz, schoolId, schoolClassIds });
  }

  public async perform(contentTreeNodeId: string): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform(contentTreeNodeId);
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const { quiz, schoolId, schoolClassIds } = canPerformResult.data;
    const quizIds = [quiz.row.quizId];

    const dialogData = new AssignLearningMaterialsAndQuizzesDialogData(
      this._strings.assignQuizToStudents,
      [],
      quizIds,
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
      quizIds,
      dialogResult.data.dueDate,
      false
    );

    if (result?.notAssignedQuizIds && result?.notAssignedQuizIds.length === 1) {
      const confirmation = await this._dialog.show(
        ConfirmationDialog,
        new ConfirmationDialogData(this._strings.quizAssignment, this._strings.messages.quizAssignmentConfirmation)
      );
      if (confirmation.action !== DialogAction.yes) {
        return Result.success();
      }

      await this._sendCommand(
        dialogResult.data.studentIdsBySchoolClassIdMap,
        result?.notAssignedQuizIds,
        dialogResult.data.dueDate,
        true
      );
    }

    return Result.success();
  }

  private async _sendCommand(
    studentIdsBySchoolClassIdMap: Record<string, string[]>,
    quizIds: string[],
    dueDateUtc: Moment,
    forceCreateNew: boolean
  ): Promise<AssignQuizzesToClassCommand_Result | null> {
    let result = null;
    try {
      result = await firstValueFrom(this._api.send(new AssignQuizzesToClassCommand(
        studentIdsBySchoolClassIdMap,
        quizIds,
        dueDateUtc,
        forceCreateNew
      )));
    } catch (e: any) {
      if (e.errors?.Error[0]) {
        this._appEvents.errorEmit(e.errors.Error[0]);
      }
    }

    return result;
  }
}
