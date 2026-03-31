import { Injectable } from '@angular/core';

import { Api } from '../../api/api.service';
import { ReassignLearningMaterialsToStudentsCommand } from '../../api/material-assignments/reassign-learning-materials-to-students.command';
import { ReassignQuizzesToStudentsCommand } from '../../api/quiz-assignments/reassign-quizzes-to-students.command';
import { Dialog } from '../../core/dialogs/dialog.service';
import { ActiveSchoolClassService } from '../../core/services/active-school-class.service';
import { ActiveSchoolService } from '../../core/services/active-school.service';
import { ContentTreeService } from '../../core/services/content/content-tree.service';
import { ErrorMessageDialogComponent } from '../../share/dialogs/error-message-dialog/error-message-dialog.component';
import { hasValue } from '../../share/helpers/has-value';
import { FailedOrSuccessResult, Result } from '../../share/helpers/result';
import { SnackBarService } from '../../share/services/snack-bar.service';
import { StringsService } from '../../share/strings.service';
import { AssignLearningMaterialsAndQuizzesDialog } from '../dialogs/assign-learning-materials-and-quizzes-dialog/assign-learning-materials-and-quizzes-dialog.component';
import { AssignLearningMaterialsAndQuizzesDialogData } from '../dialogs/assign-learning-materials-and-quizzes-dialog/assign-learning-materials-and-quizzes-dialog.data';

@Injectable({
  providedIn: 'root',
})
export class ReassignQuizzesToStudentsOperation {

  constructor(
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _strings: StringsService,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _contentTreeService: ContentTreeService,
    private readonly _snackBarService: SnackBarService,
    private readonly _activeSchoolClassService: ActiveSchoolClassService,
  ) {
  }

  async perform(studentIds: string[], quizAssignmentIds: string[], learningMaterialIds: string[], quizIds: string[]): Promise<FailedOrSuccessResult> {
    if (studentIds.length === 0) {
      this._snackBarService.showMessage(this._strings.messages.pleaseSelectStudents);
      return Result.failed();
    }

    if (quizAssignmentIds.length === 0 && learningMaterialIds.length === 0) {
      this._snackBarService.showMessage(this._strings.messages.pleaseSelectQuizzesOrLearningMaterials);
      return Result.failed();
    }

    const schoolId = await this._activeSchoolService.schoolId;
    if (!hasValue(schoolId)) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: this._strings.errors.userIsNotAssignedToSchool });
      return Result.failed();
    }

    const schoolClassId = await this._activeSchoolClassService.schoolClassId;
    if (!hasValue(schoolClassId)) {
      this._snackBarService.showMessage(this._strings.errors.classIsNotSelected);
      return Result.failed();
    }

    const dialogData = new AssignLearningMaterialsAndQuizzesDialogData(
      this._strings.reassignLearningMaterialsAndQuizzes,
      learningMaterialIds,
      quizIds,
      schoolId,
      undefined,
      studentIds,
    );

    const dialogResult = await this._dialog.show(AssignLearningMaterialsAndQuizzesDialog, dialogData);
    if (!dialogResult.hasData()) {
      return Result.failed();
    }

    const students = dialogResult.data?.studentIdsBySchoolClassIdMap[''] ?? [];

    if (quizAssignmentIds.length > 0 && students.length > 0) {
      await this._api.send(
        new ReassignQuizzesToStudentsCommand(
          quizAssignmentIds,
          students,
          dialogResult.data.dueDate)
      ).toPromise();
    }

    if (learningMaterialIds.length > 0 && students.length > 0) {
      const command = new ReassignLearningMaterialsToStudentsCommand(
        schoolClassId,
        learningMaterialIds,
        students,
        dialogResult.data.dueDate
      );
      await this._api.send(command).toPromise();
    }

    return Result.success();
  }
}
