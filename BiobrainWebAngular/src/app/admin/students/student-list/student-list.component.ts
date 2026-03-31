import { Component, OnInit } from '@angular/core';
import { combineLatest, ReplaySubject } from 'rxjs';
import { map, startWith, tap } from 'rxjs/operators';
import { ChangePasswordOperation } from 'src/app/auth/operations/change-password.operation';
import { CreateStudentService } from 'src/app/share/components/students/services/create-student.service';
import { DeleteStudentService } from 'src/app/share/components/students/services/delete-student.service';
import { EditStudentClassesOperation } from 'src/app/share/components/students/services/edit-student-classes.operation';
import { UpdateStudentService } from 'src/app/share/components/students/services/update-student.service';
import { StringsService } from 'src/app/share/strings.service';
import { ChangeEmailOperation } from 'src/app/auth/operations/change-email.operation';

import { ResetPasswordOperation } from '../../../auth/operations/reset-password.operation';
import { GetSchoolDataFromRouteService } from '../../services/get-school-data-from-route.service';
import { StudentListStore } from '../student-grid/student-list-store';
import { AddExistingStudentToSchoolOperation } from 'src/app/share/components/students/services/add-existing-student-to-school.operation';
import { UsageReportOperation } from '../../operations/usage-report.operation';

@Component({
  selector: 'app-student-list',
  templateUrl: './student-list.component.html',
  styleUrls: ['./student-list.component.scss'],
  providers: [
    StudentListStore,
    CreateStudentService,
    UpdateStudentService,
    DeleteStudentService
  ]
})
export class StudentListComponent implements OnInit {
  private _schoolId: string | undefined;
  constructor(
    public readonly strings: StringsService,
    private readonly _getSchoolDataFromRouteService: GetSchoolDataFromRouteService,
    private readonly _studentListStore: StudentListStore,
    private readonly _createStudentService: CreateStudentService,
    private readonly _updateStudentService: UpdateStudentService,
    private readonly _deleteStudentService: DeleteStudentService,
    private readonly _editStudentClassesOperation: EditStudentClassesOperation,
    private readonly _resetPasswordOperation: ResetPasswordOperation,
    private readonly _changeEmailOperation: ChangeEmailOperation,
    private readonly _changePasswordOperation: ChangePasswordOperation,
    private readonly _addExistingStudentToSchoolOperation: AddExistingStudentToSchoolOperation,
    private readonly _usageReportOperation: UsageReportOperation,
  ) {
  }

  ngOnInit(): void {
    this._studentListStore.attachBinding(
      this._getSchoolDataFromRouteService.getSchoolId().pipe(
        tap(_ => this._schoolId = _),
        map(schoolId => ({ schoolId, schoolClassId: null }))
      )
    );

    this._studentListStore.attachReload(
      combineLatest([
        this._createStudentService.createdStudent$.pipe(startWith({})),
        this._updateStudentService.updatedStudent$.pipe(startWith({})),
        this._deleteStudentService.deletedStudent$.pipe(startWith({})),
        this._addExistingStudentToSchoolOperation.addExisting$.pipe(startWith({}))
      ])
    );
  }

  onCreateStudent(): void {
    this._createStudentService.perform(this._getSchoolDataFromRouteService.getSnapshotSchoolId());
  }

  onEditStudent(studentId: string): void {
    this._updateStudentService.perform(studentId);
  }

  onDeleteStudent(studentId: string): void {
    this._deleteStudentService.perform(studentId, this._getSchoolDataFromRouteService.getSnapshotSchoolId());
  }

  async onEditStudentClasses(studentId: string): Promise<void> {
    var schoolId = this._getSchoolDataFromRouteService.getSnapshotSchoolId();
    await this._editStudentClassesOperation.perform(studentId, schoolId);
    this._studentListStore.reload();
  }

  async onResetPassword(studentId: string): Promise<void> {
    await this._resetPasswordOperation.perform(studentId);
  }

  async onChangeEmail(studentId: string): Promise<void> {
    await this._changeEmailOperation.perform(studentId);
  }

  async onChangePassword(studentId: string): Promise<void> {
    await this._changePasswordOperation.perform(studentId);
  }

  async onAddStudentByEmail(){
    var schoolId = this._getSchoolDataFromRouteService.getSnapshotSchoolId();
    await this._addExistingStudentToSchoolOperation.perform(schoolId);
  }

  async onGetUsageReport(): Promise<void> {
    if(!this._schoolId) return;
    await this._usageReportOperation.perform(this._schoolId);
  }
}
