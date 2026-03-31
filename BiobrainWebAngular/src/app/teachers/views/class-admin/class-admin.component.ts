import { Component } from '@angular/core';
import { ColDef, GridApi, GridOptions } from 'ag-grid-community';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { filter, first, map } from 'rxjs/operators';
import { studentByNameComparer } from 'src/app/api/quizzes/quiz-analytic-output.models';
import { CurrentUserService } from 'src/app/auth/services/current-user.service';
import { ActiveSchoolClassService } from 'src/app/core/services/active-school-class.service';
import { SchoolClassesCacheService } from 'src/app/core/services/school-classes/school-classes.service';
import { Student } from 'src/app/core/services/students/student';
import { ButtonCellRenderer } from 'src/app/share/components/button-cell-renderer/button-cell-renderer.component';
import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { UpdateStudentService } from 'src/app/share/components/students/services/update-student.service';
import { hasValue } from 'src/app/share/helpers/has-value';

import { StringsService } from '../../../share/strings.service';
import { getStudentEmailGetter } from '../../helpers/get-student-email-getter';
import { getStudentNameGetter } from '../../helpers/get-student-name-getter';
import { ResetPasswordOperation } from 'src/app/auth/operations/reset-password.operation';
import { RemoveStudentClassOperation } from '../../operations/remove-student-class.operation';
import { ClassStudentListStore, ClassStudentListStoreCriteria } from './class-student-list-store';
import { ActiveSchoolService } from 'src/app/core/services/active-school.service';
import {SchoolClassChangesService} from "../../school-class-changes.service";

const BUFFER_SIZE = 1;
@Component({
  selector: 'app-class-admin',
  templateUrl: './class-admin.component.html',
  styleUrls: ['./class-admin.component.scss'],
  providers: [ClassStudentListStore]
})
export class ClassAdminComponent extends DisposableSubscriberComponent {

  components = {
    buttonCellRenderer: ButtonCellRenderer
  }

  private readonly _gridApi: ReplaySubject<GridApi> = new ReplaySubject<GridApi>(BUFFER_SIZE);
  private readonly _gridApi$: Observable<GridApi> = this._gridApi.asObservable();
  private readonly _sizeGridToFit$: ReplaySubject<undefined> = new ReplaySubject<undefined>(BUFFER_SIZE);
  private currentClassId: string = "";

  constructor(
    public readonly strings: StringsService,
    _userService: CurrentUserService,
    _activeSchoolClassService: ActiveSchoolClassService,
    _schoolClassesService: SchoolClassesCacheService,
    private _studentStore: ClassStudentListStore,
    private readonly _updateStudentService: UpdateStudentService,
    private readonly _resetPasswordOperation: ResetPasswordOperation,
    private readonly _removeStudentOperation: RemoveStudentClassOperation,
    private readonly _activeSchoolService: ActiveSchoolService,
    private readonly _schoolClassChangesService: SchoolClassChangesService
  ) {
    super();

    let criteria$ = combineLatest([
      _activeSchoolService.schoolId,
      _activeSchoolClassService.schoolClassIdChanges$.pipe(filter(hasValue))
    ]).pipe(
      map(([schoolId, classId]): ClassStudentListStoreCriteria => {
        this.currentClassId = classId ?? '';
        return ({ schoolId: schoolId ?? '', schoolClassId: classId });
      })
    );

    _studentStore.attachBinding(criteria$);
    _studentStore.attachReload(_updateStudentService.updatedStudent$);
    _studentStore.attachReload(_schoolClassChangesService.changes$);

    this.pushSubscribtions(
      criteria$.subscribe(),

      this._gridApi.subscribe(x => x.sizeColumnsToFit()),

      combineLatest([_studentStore.items$.pipe(filter(hasValue)), this._gridApi$]).subscribe(([results, gridApi]) => {
        gridApi.setColumnDefs(this._buildColumns(results));
        gridApi.setRowData(this._buildRows(results));
        this._sizeGridToFit$.next();
      }),
    );
  }

  onGridReady(params: GridOptions): void {
    if (!params.api) {
      throw new Error('params.api must be defined');
    }

    this._gridApi.next(params.api);
  }

  onModelUpdated(): void {
    this._sizeGridToFit$.next();
  }

  private _buildRows(results: Student[]): Array<{ studentId: string }> {
    if (results === null) {
      return [];
    }

    return results.sort(studentByNameComparer).map(x => ({ studentId: x.studentId }));
  }

  private _buildColumns(students: Student[]): ColDef[] {
    const commonCellClass = 'ag-grid-custom-common-cell';

    const minWidth = 100;

    if (!students) {
      return [];
    }

    return [
      {
        colId: 'studentName',
        headerName: '',
        cellClass: [commonCellClass, 'ag-grid-custom-first-column-cell'],
        valueGetter: getStudentNameGetter(students),
        minWidth: minWidth,
        autoHeight: true,
        flex: 2,
      },
      {
        colId: 'email',
        headerName: '',
        cellClass: [commonCellClass, 'ag-grid-custom-first-column-cell'],
        valueGetter: getStudentEmailGetter(students),
        minWidth: minWidth,
        autoHeight: true,
        flex: 2,
      },

      {
        colId: 'edit',
        headerName: '',
        cellRenderer: 'buttonCellRenderer',
        cellRendererParams: {
          clicked: (data: any) => {
            this.onEditStudent.bind(this)(data.studentId);
          }
        },
        valueGetter: (() => this.strings.editStudent.toLocaleLowerCase()).bind(this),
        autoHeight: true,
        width: 140,
      },

      // {
      //   colId: 'moveStudent',
      //   headerName: '',
      //   cellRenderer: 'buttonCellRenderer',
      //   cellRendererParams: {
      //     clicked: (data: any) => {
      //       this.onMoveStudent.bind(this)(data.studentId);
      //     }
      //   },
      //   valueGetter: (() => this.strings.moveStudent.toLocaleLowerCase()).bind(this),
      //   suppressMovable: true,
      //   autoHeight: true,
      //   width: 160,
      // },

      {
        colId: 'resetPassword',
        headerName: '',
        cellRenderer: 'buttonCellRenderer',
        cellRendererParams: {
          clicked: (data: any) => {
            this.onResetPassword.bind(this)(data.studentId);
          }
        },
        valueGetter: (() => this.strings.resetPassword.toLocaleLowerCase()).bind(this),
        suppressMovable: true,
        autoHeight: true,
        width: 160,
      },

      {
        colId: 'remove',
        headerName: '',
        cellRenderer: 'buttonCellRenderer',
        cellRendererParams: {
          clicked: (data: any) => {
            this.onRemoveStudent.bind(this)(data.studentId);
          }
        },
        valueGetter: (() => this.strings.remove.toLocaleLowerCase()).bind(this),
        suppressMovable: true,
        autoHeight: true,
        width: 115,
      },
    ];
  }

  getStudentsCount(): Observable<number> {
    return this._studentStore.items$.pipe(
      map(x => x?.length ?? 0)
    );
  }

  async onEditStudent(studentId: string) {
    await this._updateStudentService.perform(studentId);
  }

  async onResetPassword(studentId: string) {
    await this._resetPasswordOperation.perform(studentId);
  }

  async onMoveStudent(studentId: string) {
    // await this._resetPasswordOperation.perform(studentId);
  }

  async onRemoveStudent(studentId: string) {
    var students = await this._studentStore.items$.pipe(first()).toPromise();
    let name = students.find(x => x.studentId == studentId)?.fullName ?? '';
    await this._removeStudentOperation.perform(studentId, this.currentClassId, name);
    await this._studentStore.reload();
  }
}
