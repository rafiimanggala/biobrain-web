import { Injectable } from '@angular/core';
import { Moment } from 'moment';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';

import { Api } from '../../api/api.service';
import { AssignLearningMaterialToClassCommand, AssignLearningMaterialToClassCommand_Result } from '../../api/material-assignments/assign-learning-material-to-class.command';
import { AssignQuizzesToClassCommand, AssignQuizzesToClassCommand_Result } from '../../api/quiz-assignments/assign-quizzes-to-class.command';
import { GetTeacherSchoolClassesByCourseIdQuery } from '../../api/school-classes/get-teacher-school-classes-by-course-id.query';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ActiveCourseService } from '../../core/services/active-course.service';
import { ActiveSchoolClassService } from '../../core/services/active-school-class.service';
import { ActiveSchoolService } from '../../core/services/active-school.service';
import { ContentTreeNode } from '../../core/services/content/content-tree.node';
import { ContentTreeService } from '../../core/services/content/content-tree.service';
import { LearningMaterialsService } from '../../core/services/learning-materials/learning-materials.service';
import { QuizzesService } from '../../core/services/quizzes/quizzes.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { firstValueFrom } from '../../share/helpers/first-value-from';
import { hasValue } from '../../share/helpers/has-value';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { AssignLearningMaterialsAndQuizzesDialog } from '../dialogs/assign-learning-materials-and-quizzes-dialog/assign-learning-materials-and-quizzes-dialog.component';
import { AssignLearningMaterialsAndQuizzesDialogData } from '../dialogs/assign-learning-materials-and-quizzes-dialog/assign-learning-materials-and-quizzes-dialog.data';

@Injectable({
  providedIn: 'root',
})
export class AssignLearningMaterialsAndQuizzesOperation {

  constructor(
    private readonly _dialog: Dialog,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
    private readonly _api: Api,
    private readonly _strings: StringsService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _quizzesService: QuizzesService,
    private readonly _learningMaterialsService: LearningMaterialsService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _appEvents: AppEventProvider,
    private readonly _activeCourseService: ActiveCourseService,
  ) {
  }

  public async canPerform(nodeId: string): Promise<
  SuccessOrFailedResult<{ schoolId: string; schoolClassIds: string[]; parent: ContentTreeNode },
  string>> {
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

    const node = await firstValueFrom(this._contentTreeService.getNode(nodeId));
    if (!hasValue(node)) {
      return Result.failed(this._strings.errors.contentTreeNodeWasNotFound);
    }

    const { parent } = node;
    if (!hasValue(parent)) {
      return Result.failed(this._strings.errors.nodeHasNoParent);
    }
    if (parent.children.length === 0) {
      return Result.failed(this._strings.errors.nodeHasNoChildren);
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

    return Result.success({ schoolId, schoolClassIds, parent });
  }

  public async perform(nodeId: string, levelIds: string[]): Promise<SuccessOrFailedResult> {
    const canPerformResult = await this.canPerform(nodeId);
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const { schoolId, schoolClassIds } = canPerformResult.data;

    const contentTreeNodeIds = levelIds;

    const learningMaterials = await this._learningMaterialsService.getByIds(contentTreeNodeIds).toPromise();
    const quizzes = await this._quizzesService.getByNodeIds(contentTreeNodeIds).toPromise();


    const learningMaterialIds = learningMaterials.map(_ => _.row.nodeId);
    const quizIds = quizzes.filter(_ => _.row.questions.length > 0).map(_ => _.row.quizId);

    const dialogData = new AssignLearningMaterialsAndQuizzesDialogData(
      this._strings.assignLearningMaterialsAndQuizzes,
      learningMaterialIds,
      quizIds,
      schoolId,
      schoolClassIds,
    );

    const dialogResult = await this._dialog.show(AssignLearningMaterialsAndQuizzesDialog, dialogData);
    if (!dialogResult.hasData()) {
      return Result.failed();
    }

    const selectedMaterialIds = dialogResult.data.selectedLearningMaterialIds.length > 0
      ? dialogResult.data.selectedLearningMaterialIds
      : learningMaterialIds;

    let materialsResult = null;
    if (selectedMaterialIds.length > 0 && dialogResult.data.studentIdsBySchoolClassIdMap) {
      materialsResult = await this._sendMaterialsCommand(
        dialogResult.data.studentIdsBySchoolClassIdMap,
        selectedMaterialIds,
        dialogResult.data.dueDate,
        false
      );
    }

    let quizzesResult = null;
    if (quizIds.length > 0 && dialogResult.data.studentIdsBySchoolClassIdMap) {
      quizzesResult = await this._sendQuizzesCommand(
        dialogResult.data.studentIdsBySchoolClassIdMap,
        quizIds,
        dialogResult.data.dueDate,
        false,
        dialogResult.data.hintsEnabled,
        dialogResult.data.soundEnabled
      );
    }

    if (materialsResult?.notAssignedNodeIds && materialsResult.notAssignedNodeIds.length > 0 || quizzesResult?.notAssignedQuizIds && quizzesResult.notAssignedQuizIds.length > 0) {
      const materialIds = materialsResult?.notAssignedNodeIds;
      const quizIds = quizzesResult?.notAssignedQuizIds;

      const notAssignedQuizzesNames = quizzes.filter(_ => quizIds?.includes(_.row.quizId)).map(_ => _.node.namePath[_.node.namePath.length - 1]);
      const notAssignedMaterialNames = learningMaterials.filter(_ => materialIds?.includes(_.row.nodeId)).map(_ => _.node.namePath[_.node.namePath.length - 1]);
      const confirmation = await this._dialog.show(
        ConfirmationDialog,
        new ConfirmationDialogData(
          this._strings.assignments,
          this._strings.messages.materialAndQuizAssignmentConfirmation(notAssignedQuizzesNames, notAssignedMaterialNames))
      );
      if (confirmation.action !== DialogAction.yes) return Result.success();

      if (materialsResult?.notAssignedNodeIds && materialsResult.notAssignedNodeIds.length > 1) {
        await this._sendMaterialsCommand(
          dialogResult.data.studentIdsBySchoolClassIdMap,
          materialsResult?.notAssignedNodeIds,
          dialogResult.data.dueDate,
          true
        );
      }

      if (quizzesResult?.notAssignedQuizIds && quizzesResult?.notAssignedQuizIds.length > 1) {
        await this._sendQuizzesCommand(
          dialogResult.data.studentIdsBySchoolClassIdMap,
          quizzesResult.notAssignedQuizIds,
          dialogResult.data.dueDate,
          true,
          dialogResult.data.hintsEnabled,
          dialogResult.data.soundEnabled
        );
      }
    }

    return Result.success();
  }

  private async _sendQuizzesCommand(
    studentIdsBySchoolClassIdMap: Record<string, string[]>,
    quizIds: string[],
    dueDateUtc: Moment,
    forceCreateNew: boolean,
    hintsEnabled: boolean = true,
    soundEnabled: boolean = true
  ): Promise<AssignQuizzesToClassCommand_Result | null> {
    if (Object.keys(studentIdsBySchoolClassIdMap).length === 0) {
      return null;
    }

    let result = null;
    try {
      result = await firstValueFrom(this._api.send(new AssignQuizzesToClassCommand(
        studentIdsBySchoolClassIdMap,
        quizIds,
        dueDateUtc,
        forceCreateNew,
        hintsEnabled,
        soundEnabled
      )));
    } catch (e: any) {
      if (e.errors?.Error[0]) {
        this._appEvents.errorEmit(e.errors.Error[0]);
      }
    }

    return result;
  }

  private async _sendMaterialsCommand(
    studentIdsBySchoolClassIdMap: Record<string, string[]>,
    learningMaterialIds: string[],
    dueDateUtc: Moment,
    forceCreateNew: boolean
  ): Promise<AssignLearningMaterialToClassCommand_Result | null> {
    if (Object.keys(studentIdsBySchoolClassIdMap).length === 0) {
      return null;
    }

    let result = null;
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
