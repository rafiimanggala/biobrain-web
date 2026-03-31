import { Component, OnInit } from '@angular/core';
import { combineLatest } from 'rxjs';
import { map, startWith, tap } from 'rxjs/operators';
import { AddTeacherWithEmailOperation } from 'src/app/auth/operations/add-teacher-with-email.operation';
import { ChangePasswordOperation } from 'src/app/auth/operations/change-password.operation';
import { StringsService } from 'src/app/share/strings.service';
import { ChangeEmailOperation } from '../../../auth/operations/change-email.operation';

import { ResetPasswordOperation } from '../../../auth/operations/reset-password.operation';
import { GetSchoolDataFromRouteService } from '../../services/get-school-data-from-route.service';
import { CreateTeacherService } from '../services/create-teacher.service';
import { DeleteTeacherService } from '../services/delete-teacher.service';
import { EditTeacherClassesOperation } from '../services/edit-teacher-classes-operation.service';
import { UpdateTeacherDetailsService } from '../services/update-teacher-details.service';
import { TeacherListStore } from '../teacher-grid/teacher-list-store';
import { UsageReportOperation } from '../../operations/usage-report.operation';

@Component({
  selector: 'app-teacher-list',
  templateUrl: './teacher-list.component.html',
  styleUrls: ['./teacher-list.component.scss'],
  providers: [
    TeacherListStore,
    CreateTeacherService,
    UpdateTeacherDetailsService,
    DeleteTeacherService,
    EditTeacherClassesOperation
  ],
})
export class TeacherListComponent implements OnInit {

  private _schoolId: string | undefined;

  constructor(
    public readonly strings: StringsService,
    private readonly _getSchoolDataFromRouteService: GetSchoolDataFromRouteService,
    private readonly _teacherListStore: TeacherListStore,
    private readonly _createTeacherService: CreateTeacherService,
    private readonly _updateTeacherDetailsService: UpdateTeacherDetailsService,
    private readonly _deleteTeacherService: DeleteTeacherService,
    private readonly _editTeacherClassesOperation: EditTeacherClassesOperation,
    private readonly _resetPasswordOperation: ResetPasswordOperation,
    private readonly _changeEmailOperation: ChangeEmailOperation,
    private readonly _changePasswordOperation: ChangePasswordOperation,
    private readonly _addTeacherWithEmailOperation: AddTeacherWithEmailOperation,
    private readonly _usageReportOperation: UsageReportOperation,
  ) {
  }

  ngOnInit(): void {
    this._teacherListStore.attachBinding(
      this._getSchoolDataFromRouteService.getSchoolId().pipe(tap(_ => this._schoolId = _), map(schoolId => ({ schoolId })))
    );

    this._teacherListStore.attachReload(
      combineLatest([
        this._createTeacherService.createdTeacher$.pipe(startWith({})),
        this._updateTeacherDetailsService.updatedTeacher$.pipe(startWith({})),
        this._deleteTeacherService.deletedTeacher$.pipe(startWith({})),
        this._addTeacherWithEmailOperation.addTeacher$.pipe(startWith({})),
      ])
    );
  }

  onCreateTeacher(): void {
    this._createTeacherService.perform(this._getSchoolDataFromRouteService.getSnapshotSchoolId());
  }

  onAddTeacherByEmail(): void {
    this._addTeacherWithEmailOperation.perform(this._getSchoolDataFromRouteService.getSnapshotSchoolId());
  }

  onEditTeacher(teacherId: string): void {
    this._updateTeacherDetailsService.perform(teacherId);
  }

  onDeleteTeacher(teacherId: string): void {
    this._deleteTeacherService.perform(teacherId, this._getSchoolDataFromRouteService.getSnapshotSchoolId());
  }

  onEditTeacherClasses(teacherId: string): void {
    this._editTeacherClassesOperation.perform(teacherId, this._getSchoolDataFromRouteService.getSnapshotSchoolId());
  }

  async onResetPassword(teacherId: string): Promise<void> {
    await this._resetPasswordOperation.perform(teacherId);
  }

  async onChangeEmail(teacherId: string): Promise<void> {
    await this._changeEmailOperation.perform(teacherId);
  }

  async onChangePassword(teacherId: string): Promise<void> {
    await this._changePasswordOperation.perform(teacherId);
  }

  async onGetUsageReport(): Promise<void> {
    if(!this._schoolId) return;
    await this._usageReportOperation.perform(this._schoolId);
  }
}
