import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { GridApi, GridOptions, ValueFormatterParams } from 'ag-grid-community';
import { Subscription } from 'rxjs';
import { StringsService } from 'src/app/share/strings.service';
import { AgGridSettings } from 'src/app/share/values/ag-grid-settings';

import { StudentListStore } from './student-list-store';


@Component({
  selector: 'app-student-grid',
  templateUrl: './student-grid.component.html',
  styleUrls: ['./student-grid.component.scss']
})
export class StudentGridComponent implements OnInit, OnDestroy {
  @Output() editStudent = new EventEmitter<string>();
  @Output() editStudentClasses = new EventEmitter<string>();
  @Output() deleteStudent = new EventEmitter<string>();
  @Output() resetPassword = new EventEmitter<string>();
  @Output() changeEmail = new EventEmitter<string>();
  @Output() changePassword = new EventEmitter<string>();

  textFilterParams = AgGridSettings.textFilterParams;

  private readonly _subscriptions: Subscription[] = [];
  private _gridApi: GridApi | null | undefined;

  joinClassNames = (params: ValueFormatterParams): string => (params.value as string[]).join(', ');

  constructor(
    public readonly studentListStore: StudentListStore,
    public readonly strings: StringsService
  ) { }

  ngOnInit(): void {
    this._subscriptions.push(
      this.studentListStore.items$.subscribe(() =>
        this._gridApi?.sizeColumnsToFit()
      )
    );
  }

  ngOnDestroy(): void {
    this._subscriptions.forEach(s => s.unsubscribe());
  }

  onGridReady(params: GridOptions): void {
    this._gridApi = params.api;
    this._gridApi?.sizeColumnsToFit();
  }

  onModelUpdated(): void {
    this._gridApi?.sizeColumnsToFit();
  }

  onEditStudent(studentId: string): void {
    this.editStudent.next(studentId);
  }

  onDeleteStudent(studentId: string): void {
    this.deleteStudent.next(studentId);
  }

  onEditStudentClasses(studentId: string): void {
    this.editStudentClasses.next(studentId);
  }

  onResetPassword(studentId: string): void {
    this.resetPassword.next(studentId);
  }

  onChangeEmail(studentId: string): void {
    this.changeEmail.next(studentId);
  }

  onChangePassword(teacherId: string): void {
    this.changePassword.next(teacherId);
  }
}
