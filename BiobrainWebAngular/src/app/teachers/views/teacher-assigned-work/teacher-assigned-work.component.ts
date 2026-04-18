import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, ColGroupDef, GridApi, GridOptions, ValueGetterParams } from 'ag-grid-community';
import moment, { Moment } from 'moment';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { distinctUntilChanged, filter, map, switchMap, tap } from 'rxjs/operators';
import { ContentTreeMetaListStore } from 'src/app/admin/content/content-mapper/content-tree-meta-list-store';
import { ActiveLearningMaterialClassAssignment, ActiveQuizClassAssignment, GetTeacherAssignedWorkQuery, TeacherAssignedWork } from 'src/app/api/assigned-work/get-teacher-assigned-work.query';
import { GetContentTreeMetaListQuery_Result } from 'src/app/api/content/get-content-tree-meta-list.query';
import { AssignmentStatus } from 'src/app/api/enums/assignment-status.enum';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { ActiveSchoolClassService } from 'src/app/core/services/active-school-class.service';
import { ButtonCellRenderer } from 'src/app/share/components/button-cell-renderer/button-cell-renderer.component';
import { EditableCellRenderer } from 'src/app/share/components/editable-cell-renderer/editable-cell-renderer.component';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { numberToStringWithLength } from 'src/app/share/helpers/number-to-string-with-length';

import { Api } from '../../../api/api.service';
import { CurrentUserService } from '../../../auth/services/current-user.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { BaseComponent } from '../../../core/app/base.component';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { assertHasValue } from '../../../share/helpers/assert-has-value';
import { capitalizeFirstLetter } from '../../../share/helpers/capitalize-first-letter';
import { hasValue } from '../../../share/helpers/has-value';
import { StringsService } from '../../../share/strings.service';
import { UnassignMaterialOperation } from '../../operations/unassign-material.operation';
import { UnassignQuizOperation } from '../../operations/unassign-quiz.operation';
import { UpdateDueDateForMaterialOperation } from '../../operations/update-due-date-for-material.operation';
import { UpdateDueDateForQuizOperation } from '../../operations/update-due-date-for-quiz.operation';


const ACTION_COLUMN = 'action_column';

type RowModel = ActiveQuizClassAssignment | ActiveLearningMaterialClassAssignment;

@Component({
  selector: 'app-teacher-assigned-work',
  templateUrl: './teacher-assigned-work.component.html',
  styleUrls: ['./teacher-assigned-work.component.scss'],
  providers: [UpdateDueDateForMaterialOperation, UpdateDueDateForQuizOperation, UnassignMaterialOperation, UnassignQuizOperation, ContentTreeMetaListStore]
})
export class TeacherAssignedWorkComponent extends BaseComponent implements OnInit {
  @ViewChild('agGridRef') agGrid?: AgGridAngular;
  @ViewChild('agGridRef', { read: ElementRef }) agGridElement?: ElementRef;

  components = {
    buttonCellRenderer: ButtonCellRenderer,
    editableCellRenderer: EditableCellRenderer,
  }
  private readonly _gridApi = new ReplaySubject<GridApi>(1);
  private readonly _gridApi$ = this._gridApi.asObservable();
  private readonly _sizeGridToFit$ = new ReplaySubject<undefined>(1);
  private readonly _activeClassId$: Observable<string>;
  private _userId: string = '';
  private _schoolClassId: string = '';

  subjectName: string = '';
  courseId: string = '';

  constructor(
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _activeClassService: ActiveSchoolClassService,
    private readonly _api: Api,
    private readonly _currentUserService: CurrentUserService,
    private readonly _routingService: RoutingService,
    private readonly _updateDueDateForMaterialOperation: UpdateDueDateForMaterialOperation,
    private readonly _updateDueDateForQuizOperation: UpdateDueDateForQuizOperation,
    private readonly _unassignMaterialOperation: UnassignMaterialOperation,
    private readonly _unassignQuizOperation: UnassignQuizOperation,
    private readonly _contentTreeMetaListStore: ContentTreeMetaListStore,
    public strings: StringsService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);

    this._activeClassId$ = this._activeClassService.schoolClassIdChanges$.pipe(
      filter(hasValue),
      tap(_ => this._schoolClassId = _),
      distinctUntilChanged(),
    );

    var activeCourseId$ = this._activeCourseService.courseIdChanges$.pipe(
      filter(hasValue),
      distinctUntilChanged(),
    );

    this._contentTreeMetaListStore.attachBinding(activeCourseId$.pipe(
      map(x => ({ courseId: x }))
    ));

    this.pushSubscribtions(
      this._activeCourseService.courseChanges$.pipe(
        distinctUntilChanged(),
        tap(x => {
          this.subjectName = x?.subject?.name ?? '';
          this.courseId = x?.courseId ?? '';
        }),
      ).subscribe()
    );
  }

  ngOnInit(): void {
    const userId$ = this._currentUserService.userChanges$.pipe(
      filter(hasValue),
      map(x => x.userId),
      tap(x => this._userId = x),
      distinctUntilChanged()
    );

    const assignedWork$ = combineLatest([userId$, this._activeClassId$]).pipe(
      switchMap(([userId, classId]) => this._api.send(new GetTeacherAssignedWorkQuery(userId, classId)))
    );

    const contentMeta$ = this._contentTreeMetaListStore.items$.pipe(
      map(x => x.sort((a, b) => a.depth - b.depth))
    );

    this.pushSubscribtions(
      this._gridApi.subscribe(x => tuneGrid(x)),

      combineLatest([this._sizeGridToFit$, this._gridApi$])
        .pipe(map(([_, gridApi]) => gridApi))
        .subscribe(x => tuneGrid(x)),

      combineLatest([this._gridApi$, contentMeta$, assignedWork$]).subscribe(
        ([gridApi, contentMeta, assignedWork]) => {
          gridApi.setColumnDefs(this._buildColumns(contentMeta));
          gridApi.setRowData(convertAssignedWorkToRows(assignedWork));
          tuneGrid(gridApi);
        }
      )
    );
  }

  onGridReady(params: GridOptions): void {
    if (!params.api) {
      throw new Error('params.api must be defined');
    }

    this._gridApi.next(params.api);
    this._recalculateHeaderHeight();
  }

  onColumnResized(_params: GridOptions): void {
    this._recalculateHeaderHeight();
  }

  async onModelUpdated() {
    tuneGrid(await firstValueFrom(this._gridApi$));
  }

  onCourseClick() {
    if (!this.courseId) return;
    this._routingService.navigateToMaterialPage(this.courseId, undefined, undefined);
  }

  async onUnassignClicked(data: RowModel): Promise<void> {
    if (!data) return;

    if ('quizAssignmentId' in data) {
      var result = await this._unassignQuizOperation.perform(data.quizAssignmentId);
      if (!result.isSuccess()) {
        return;
      }
      await this._updateDataInternal();
    } else if ('learningMaterialAssignmentId' in data) {
      var result = await this._unassignMaterialOperation.perform(data.learningMaterialAssignmentId);
      if (!result.isSuccess()) {
        return;
      }
      await this._updateDataInternal();
    } else {
      throw Error('Either quizAssignmentId or learningMaterialUserAssignmentId must be specified for the row');
    }
  }

  async onQuizEditDueDate(quizAssignmentId: string, dueDate: Moment) {
    var result = await this._updateDueDateForQuizOperation.perform(quizAssignmentId, dueDate);
    if (!result.isSuccess()) {
      return;
    }
    await this._updateDataInternal();
  }

  async onMaterialEditDueDate(materialAssignmentId: string, dueDate: Moment) {
    var result = await this._updateDueDateForMaterialOperation.perform(materialAssignmentId, dueDate);
    if (!result.isSuccess()) {
      return;
    }
    await this._updateDataInternal();
  }

  private async _updateDataInternal() {
    if (!this._userId || !this._schoolClassId) return;
    var result = await firstValueFrom(this._api.send(new GetTeacherAssignedWorkQuery(this._userId, this._schoolClassId)));
    var grid = await firstValueFrom(this._gridApi);
    grid.setRowData(convertAssignedWorkToRows(result));
  }

  private _buildColumns(contentMeta: GetContentTreeMetaListQuery_Result[]): (ColDef | ColGroupDef)[] {
    const commonCellClass = 'ag-grid-custom-common-cell';
    const simpleHeaderClasses = ['ag-grid-custom-header-cell-accent-background', commonCellClass];
    const leftAlignedHeaderClasses = ['ag-grid-custom-header-cell-allign-left', ...simpleHeaderClasses];
    const centerCellClasses = [commonCellClass, 'ag-centered-cell'];
    const accentCellClasses = ['ag-grid-custom-accent-cell'];
    var columns: (ColDef | ColGroupDef)[] = [];

    contentMeta.filter(_ =>
      // ToDo: If need to exclude more columns, need to develope a proper way
      !_.name.toLocaleLowerCase().includes("organization")
    ).sort((a, b) => a.depth - b.depth).map(x => ({
      colId: `${numberToStringWithLength(x.depth, 3)}-content-tree`,
      headerName: `${capitalizeFirstLetter(x.name)}`,
      headerClass: simpleHeaderClasses,
      cellClass: [...centerCellClasses, ...this.getClasses(x.name)],
      valueGetter: getDepth(x.depth, x.name),
      suppressMovable: true,
      autoHeight: true,
      wrapText: true,
      flex: getFlex(x.name, x.depth),
      minWidth: getMinWidth(x.name),
    })).forEach(x => columns.push(x));
    var maxIndex = columns.length;

    columns.push(...
      [
        // {
        //   colId: `${0}-work-assigned`,
        //   headerName: `${capitalizeFirstLetter(this.strings.workAssigned)}`,
        //   headerClass: [...simpleHeaderClasses, 'ag-grid-custom-first-column-header'],
        //   cellClass: [aggregateCellClass, 'ag-grid-custom-first-column-cell'],
        //   valueGetter: (params: ValueGetterParams) => (params.node?.data as RowModel)?.title,
        //   suppressMovable: true,
        //   minWidth: 240,
        //   autoHeight: true,
        //   wrapText: true,
        //   flex: 2,
        // },
        {
          colId: `${numberToStringWithLength((maxIndex + 1), 3)}-student-number`,
          headerName: `${capitalizeFirstLetter(this.strings.numberOfStudentsAssigned)}`,
          headerClass: [...simpleHeaderClasses],
          cellClass: centerCellClasses,
          field: "studentAssigned",
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          minWidth: 100,
          flex: 1,
        },
        {
          colId: `${numberToStringWithLength((maxIndex + 2), 3)}-status`,
          headerName: `${capitalizeFirstLetter(this.strings.status)}`,
          headerClass: [...simpleHeaderClasses],
          cellClass: centerCellClasses,
          valueGetter: this._getStatusColumnText(),
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          minWidth: 140,
          flex: 1.2,
        },
        {
          colId: `${numberToStringWithLength((maxIndex + 3), 3)}-type`,
          headerName: `${capitalizeFirstLetter(this.strings.type)}`,
          headerClass: [...simpleHeaderClasses],
          cellClass: centerCellClasses,
          valueGetter: this._getTypeColumnText(),
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          minWidth: 80,
          flex: 1,
        },
        {
          colId: `${numberToStringWithLength((maxIndex + 4), 3)}-assignedAt`,
          headerName: `${capitalizeFirstLetter(this.strings.dateAssigned)}`,
          headerClass: [...simpleHeaderClasses],
          cellClass: centerCellClasses,
          valueGetter: (params: ValueGetterParams) => formatDate((params.node?.data as RowModel)?.assignedAt),
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          minWidth: 160,
          flex: 1,
        },
        {
          colId: `${numberToStringWithLength((maxIndex + 5), 3)}-dueAt`,
          headerName: `${capitalizeFirstLetter(this.strings.dueDate)}`,
          headerClass: [...leftAlignedHeaderClasses],
          cellClass: accentCellClasses,
          valueGetter: (params: ValueGetterParams) => formatDate((params.node?.data as RowModel)?.dueAt),
          cellRenderer: 'editableCellRenderer',
          cellRendererParams: {
            clicked: (data: any) => {
              if ('quizAssignmentId' in data && data.dueAt) {
                this.onQuizEditDueDate.bind(this)(data.quizAssignmentId, moment.utc(data.dueAt).local());
              }

              if ('learningMaterialAssignmentId' in data && data.dueAt) {
                this.onMaterialEditDueDate.bind(this)(data.learningMaterialAssignmentId, moment.utc(data.dueAt).local());
              }
            },
            width: 120
          },
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          minWidth: 160,
          flex: 1.2,
        },
        {
          colId: `${numberToStringWithLength((maxIndex + 6), 3)}-${ACTION_COLUMN}`,
          headerName: '',
          headerClass: [...simpleHeaderClasses],
          cellClass: centerCellClasses,
          cellRenderer: 'buttonCellRenderer',
          cellRendererParams: {
            clicked: (data: any) => {
              this.onUnassignClicked.bind(this)(data);
            },
            width: 120,
            isDisabled: (data: RowModel) => data.status == AssignmentStatus.Complete
          },
          valueGetter: () => capitalizeFirstLetter(this.strings.unassign),
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          minWidth: 160,
          flex: 1.2,
        },
      ]);

    return columns.sort((a: ColDef, b: ColDef) => a?.colId?.localeCompare(b?.colId ?? '') ?? 0);
  }

  getClasses(name: string) {
    if (name.toLocaleLowerCase().includes("key knowledge"))
      return ['ag-grid-wraped-cell'];
    if (name.toLocaleLowerCase().includes("topic"))
      return ['ag-grid-wraped-cell'];
    return [];
  }

  private _getTypeColumnText(): (params: ValueGetterParams) => string {
    return params => {
      const data = params.node?.data as RowModel | null | undefined;

      if (!data) {
        return '';
      }

      if ('quizAssignmentId' in data) {
        return capitalizeFirstLetter(this.strings.quiz);
      }

      if ('learningMaterialAssignmentId' in data) {
        return capitalizeFirstLetter(this.strings.content);
      }

      throw Error('Either quizAssignmentId or learningUserAssignmentId must be specified for the row');
    };
  }

  private _getStatusColumnText(): (params: ValueGetterParams) => string {
    return params => {
      const data = params.node?.data as RowModel | null | undefined;

      if (!data) {
        return '';
      }

      switch (data.status) {
        case AssignmentStatus.Assigned: return capitalizeFirstLetter(this.strings.assigned);
        case AssignmentStatus.Complete: return capitalizeFirstLetter(this.strings.complete);
        case AssignmentStatus.PartiallyComplete: return capitalizeFirstLetter(this.strings.started);
        default: return "";
      }
    };
  }

  private _recalculateHeaderHeight(): void {
    if (!hasValue(this.agGridElement) || !hasValue(this.agGrid)) return;

    // eslint-disable-next-line @typescript-eslint/no-unsafe-call, @typescript-eslint/no-unsafe-member-access
    const headerCells = this.agGridElement.nativeElement.querySelectorAll('.custom-header-label') as NodeList;
    let maxHeight = 48;
    headerCells.forEach((cell: any) => {
      // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
      maxHeight = Math.max(maxHeight, cell.scrollHeight);
    });

    this.agGrid.api.setHeaderHeight(maxHeight + 20);
  }
}

function formatDate(date: Date | null | undefined): string {
  if (!date) return '—';

  return moment.utc(date).local().format('DD MMM YY');
}

function convertAssignedWorkToRows(assignedWork: TeacherAssignedWork): RowModel[] {
  const result: RowModel[] = [];
  result.push(...assignedWork.quizzes);
  result.push(...assignedWork.learningMaterials);

  result
  .sort((x1, x2) => {return ('quizAssignmentId' in x1) ? 1 : -1;})
  .sort((x1, x2) => {return x1.title.localeCompare(x2.title)})
  .sort((x1, x2) => {
    const dueDate1 = x1.dueAt ? moment.utc(x1.dueAt).local().startOf('day') : null;
    const dueDate2 = x2.dueAt ? moment.utc(x2.dueAt).local().startOf('day') : null;

    if (dueDate1 && dueDate2 && dueDate1.valueOf() !== dueDate2.valueOf()) {
      return dueDate2.valueOf() < dueDate1.valueOf() ? 1 : -1;
    }
    if (!dueDate1 && dueDate2) return 1;
    if (dueDate1 && !dueDate2) return -1;

    const assignedDate1 = x1.assignedAt ? moment.utc(x1.assignedAt).local().startOf('day') : null;
    const assignedDate2 = x2.assignedAt ? moment.utc(x2.assignedAt).local().startOf('day') : null;
    if (assignedDate1 && assignedDate2 && assignedDate1.valueOf() !== assignedDate2.valueOf()) {
      return assignedDate1.valueOf() < assignedDate2.valueOf() ? 1 : -1;
    }

    return x1.nodeId.localeCompare(x2.nodeId);
  });

  return result;
}

function tuneGrid(gridApi: GridApi): void {
  // gridApi.sizeColumnsToFit();
  gridApi.resetRowHeights();
}

function getDepth(depth: number, columnName?: string): (params: ValueGetterParams) => string {
  return params => {
    const data = params.node?.data as ({ path: string[]; isCustomQuiz?: boolean } | undefined | null);
    if (!data) {
      return '';
    }

    if (!data.path) {
      throw Error('Path is required for "action" column');
    }

    if (data.isCustomQuiz && columnName) {
      const override = getCustomQuizOverride(columnName);
      if (override !== null) return override;
    }

    return data.path[depth];
  };
}

function getCustomQuizOverride(name: string): string | null {
  const lower = name.toLocaleLowerCase();
  if (lower.includes('key knowledge')) return 'Custom Quiz';
  if (lower.includes('topic') && !lower.includes('subtopic')) return 'Various';
  if (lower.includes('level')) return '-';
  return null;
}

function getFlex(name: string, depth: number): number {
  let flex = 1;
  if (name.toLocaleLowerCase().includes("unit"))
    flex = 0.8;
  if (name.toLocaleLowerCase().includes("level"))
    flex = 0.8;
  if (name.toLocaleLowerCase().includes("area"))
    flex = 1.2;
  if (name.toLocaleLowerCase().includes("module"))
    flex = 0.8;
  if (name.toLocaleLowerCase().includes("content"))
    flex = 3;
  if (name.toLocaleLowerCase().includes("key knowledge"))
    flex = 3;
  if (name.toLocaleLowerCase().includes("heading"))
    flex = 3;
  if (name.toLocaleLowerCase().includes("subtopic"))
    flex = 3;
  if (name.toLocaleLowerCase().includes("topic") && !name.toLocaleLowerCase().includes("subtopic")) {
    if (depth = 0)
      flex = 0.5;
    else
      flex = 2;
  }

  return flex;
}

function getMinWidth(name: string): number {
  let minWidth = 50;
  if (name.toLocaleLowerCase().includes("unit"))
    minWidth = 50;
  if (name.toLocaleLowerCase().includes("area"))
    minWidth = 50;
  if (name.toLocaleLowerCase().includes("key knowledge"))
    minWidth = 120;
  if (name.toLocaleLowerCase().includes("topic"))
    minWidth = 120;

  return minWidth;
}
