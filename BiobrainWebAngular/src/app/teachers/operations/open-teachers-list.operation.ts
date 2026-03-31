import { Injectable } from '@angular/core';
import { GetTeacherListItemsQuery_Result } from 'src/app/api/teachers/get-teacher-list-items.query';

import { Dialog } from '../../core/dialogs/dialog.service';
import { Result, SuccessOrFailedResult } from '../../share/helpers/result';
import { StringsService } from '../../share/strings.service';
import { TeacherListDialog } from '../dialogs/teacher-list/teacher-list.dialog';

@Injectable({
  providedIn: 'root',
})
export class OpenTeachersListOperation {

  constructor(
    public readonly strings: StringsService,
    private readonly _dialog: Dialog,
  ) {
  }

  public async perform(teachers: GetTeacherListItemsQuery_Result[], classId: string): Promise<SuccessOrFailedResult> {
    await this._dialog.show(TeacherListDialog, {
      teachers: teachers,
      classId: classId
    });
    return Result.success();
  }
}
