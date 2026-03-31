import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { GridApi, GridOptions } from 'ag-grid-community';
import { Subscription } from 'rxjs';
import { StringsService } from 'src/app/share/strings.service';

import { TeacherListStore } from './teacher-list-store';

@Component({
  selector: 'app-teacher-grid',
  templateUrl: './teacher-grid.component.html',
  styleUrls: ['./teacher-grid.component.scss'],
})
export class TeacherGridComponent implements OnInit, OnDestroy {
  @Output() editTeacher = new EventEmitter<string>();
  @Output() deleteTeacher = new EventEmitter<string>();
  @Output() editTeacherClasses = new EventEmitter<string>();
  @Output() resetPassword = new EventEmitter<string>();
  @Output() changeEmail = new EventEmitter<string>();
  @Output() changePassword = new EventEmitter<string>();

  textFilterParams = {
    filterOptions: ['contains', 'notContains'],
    trimInput: true,
    debounceMs: 200,
  };

  private readonly _subscriptions: Subscription[] = [];
  private _gridApi: GridApi | null | undefined;

  constructor(
    public readonly teacherListStore: TeacherListStore,
    public readonly strings: StringsService
  ) {}

  ngOnInit(): void {
    this._subscriptions.push(
      this.teacherListStore.items$.subscribe(() =>
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

  onEditTeacher(teacherId: string): void {
    this.editTeacher.next(teacherId);
  }

  onDeleteTeacher(teacherId: string): void {
    this.deleteTeacher.next(teacherId);
  }

  onEditTeacherClasses(teacherId: string): void {
    this.editTeacherClasses.emit(teacherId);
  }

  onResetPassword(teacherId: string): void {
    this.resetPassword.next(teacherId);
  }

  onChangeEmail(teacherId: string): void {
    this.changeEmail.next(teacherId);
  }

  onChangePassword(teacherId: string): void {
    this.changePassword.next(teacherId);
  }
}
