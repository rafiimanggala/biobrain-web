import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GetTeacherListItemsQuery_Result } from 'src/app/api/teachers/get-teacher-list-items.query';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { StringsService } from 'src/app/share/strings.service';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { DeleteTeacherOperation } from '../../operations/delete-teacher.operation';
import { TeacherListDialogData } from './teacher-list.dialog-data';

@Component({
  selector: 'teacher-list-dialog',
  templateUrl: 'teacher-list.dialog.html',
  styleUrls: ['teacher-list.dialog.scss'],
})
export class TeacherListDialog extends DialogComponent<TeacherListDialogData, TeacherListDialogData> {
  currentUserId: string = '';

  constructor(
    public readonly strings: StringsService,
    public readonly currentUserService: CurrentUserService,
    private readonly deleteTeacherOperation: DeleteTeacherOperation,
    @Inject(MAT_DIALOG_DATA) public dialogData: TeacherListDialogData,
  ) {
    super(dialogData);
    currentUserService.user.then((user) => this.currentUserId = user?.userId ?? '');
  }

  onClose(): void {
    this.close(DialogAction.cancel);
  }

  async onDelete(teacher: GetTeacherListItemsQuery_Result){
    var result = await this.deleteTeacherOperation.perform(this.dialogData.classId, teacher.teacherId, teacher.fullName);
    if(!result.isSuccess()) return;
    var teacherId = result.data;
    this.dialogData.teachers.splice(this.dialogData.teachers.findIndex(x => x.teacherId == teacherId), 1);
  }
}
