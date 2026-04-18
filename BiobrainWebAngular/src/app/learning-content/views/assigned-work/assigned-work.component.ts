import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CellClickedEvent, ColDef, ColGroupDef, GridApi, GridOptions, ICellRendererParams, ValueGetterParams } from 'ag-grid-community';
import moment, { Moment } from 'moment';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { distinctUntilChanged, filter, map, pluck, switchMap, tap } from 'rxjs/operators';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { ButtonCellRenderer } from 'src/app/share/components/button-cell-renderer/button-cell-renderer.component';
import { ContentCardAction } from 'src/app/share/components/content-card/content-card-action';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';

import { ContentTreeMetaListStore } from '../../../admin/content/content-mapper/content-tree-meta-list-store';
import { Api } from '../../../api/api.service';
import {
  ActiveLearningMaterialAssignment,
  ActiveQuizAssignment,
  AssignedWork,
  GetAssignedWorkQuery
} from '../../../api/assigned-work/get-assigned-work.query';
import { GetContentTreeMetaListQuery_Result } from '../../../api/content/get-content-tree-meta-list.query';
import { CurrentUserService } from '../../../auth/services/current-user.service';
import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { BaseComponent } from '../../../core/app/base.component';
import { ActiveCourseService } from '../../../core/services/active-course.service';
import { assertHasValue } from '../../../share/helpers/assert-has-value';
import { capitalizeFirstLetter } from '../../../share/helpers/capitalize-first-letter';
import { hasValue } from '../../../share/helpers/has-value';
import { toNonNullableWithError } from '../../../share/helpers/to-non-nullable';
import { StringsService } from '../../../share/strings.service';
import { CompleteLearningMaterialAssignmentOperation } from '../../operations/complete-learning-material-assignment.operation';
import { TakeQuizOperation } from '../../operations/take-quiz.operation';


const ACTION_COLUMN = 'ACTION_COLUMN';

type RowModel = ActiveQuizAssignment | ActiveLearningMaterialAssignment;

@Component({
  selector: 'app-assigned-work',
  templateUrl: './assigned-work.component.html',
  styleUrls: ['./assigned-work.component.scss'],
  providers: [ContentTreeMetaListStore]
})
export class AssignedWorkComponent extends BaseComponent implements OnInit {

  components = {
    buttonCellRenderer: ButtonCellRenderer
  }

  private readonly _gridApi = new ReplaySubject<GridApi>(1);
  private readonly _gridApi$ = this._gridApi.asObservable();
  private readonly _sizeGridToFit$ = new ReplaySubject<undefined>(1);
  private readonly _activeCourseId$: Observable<string>;
  private readonly _updateInterval: any;
  gridApi: GridApi | null = null;
  subjectName: string = '';
  courseId: string = '';
  userId: string = '';
  rows: RowModel[] = [];

  constructor(
    private readonly _activeCourseService: ActiveCourseService,
    private readonly _contentTreeMetaListStore: ContentTreeMetaListStore,
    private readonly _takeQuizOperation: TakeQuizOperation,
    private readonly _completeLearningMaterialAssignmentOperation: CompleteLearningMaterialAssignmentOperation,
    private readonly _route: ActivatedRoute,
    private readonly _api: Api,
    private readonly _currentUserService: CurrentUserService,
    private readonly _routingService: RoutingService,
    private readonly _datePipe: DatePipe,
    public strings: StringsService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);

    this._activeCourseId$ = this._activeCourseService.courseIdChanges$.pipe(
      filter(hasValue),
      distinctUntilChanged(),
    );

    this._contentTreeMetaListStore.attachBinding(this._activeCourseId$.pipe(
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

    this._updateInterval = setInterval(async () => {
      if (!this.userId || !this.courseId) return;
      let assignedWork = await firstValueFrom(this._api.send(new GetAssignedWorkQuery(this.userId, this.courseId)));
      this.rows = convertAssignedWorkToRows(assignedWork);
      this.gridApi?.setRowData(this.rows);
      this._sizeGridToFit$.next();
    }, 60 * 1000,);
  }

  ngOnInit(): void {
    const userId$ = this._currentUserService.userChanges$.pipe(
      filter(hasValue),
      map(x => x.userId),
      tap(x => this.userId = x ?? ''),
      distinctUntilChanged()
    );

    const rootNodeId$ = this._route.params.pipe(
      pluck('rootContentNodeId'),
      filter(x => hasValue(x)),
      map(x => x as string),
      distinctUntilChanged()
    );

    const assignedWork$ = combineLatest([rootNodeId$, userId$, this._activeCourseId$]).pipe(
      switchMap(([rootNodeId, userId, courseId]) => this._api.send(new GetAssignedWorkQuery(userId, courseId)))
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
          this.rows = convertAssignedWorkToRows(assignedWork);
          gridApi.setRowData(this.rows);
          this._sizeGridToFit$.next();
        }
      )
    );
  }

  ngOnDestroy(): void {
    super.ngOnDestroy();
    clearInterval(this._updateInterval);
  }

  onGridReady(params: GridOptions): void {
    if (!params.api) {
      throw new Error('params.api must be defined');
    }

    this.gridApi = params.api;
    this._gridApi.next(params.api);
  }

  onModelUpdated(): void {
    this._sizeGridToFit$.next();
  }

  onCourseClick(): void {
    if (!this.courseId) return;
    void this._routingService.navigateToMaterialPage(this.courseId, undefined, undefined);
  }

  async onCellClicked(params: CellClickedEvent): Promise<void> {
    const senderId = params.column.getId();
    if (!senderId.includes(ACTION_COLUMN)) {
      return;
    }

    const data = params.node.data as RowModel;
    await this._handleAssignedWorkClick(data);
  }

  private async _handleAssignedWorkClick(data: RowModel): Promise<void> {
    if ('quizStudentAssignmentId' in data) {
      const courseId = toNonNullableWithError(this.strings.errors.courseMustBeSelected)(await this._activeCourseService.courseId);
      await this._takeQuizOperation.performAssignedQuiz(courseId, data.nodeId, data.quizStudentAssignmentId);
    } else if ('learningMaterialUserAssignmentId' in data) {
      await this._completeLearningMaterialAssignmentOperation.perform(data.learningMaterialUserAssignmentId, data.nodeId);
    } else {
      throw Error('Either quizAssignmentId or learningMaterialUserAssignmentId must be specified for the row');
    }
  }

  private _buildColumns(contentMeta: GetContentTreeMetaListQuery_Result[]): (ColDef | ColGroupDef)[] {
    const buttonCellClass = 'ag-grid-custom-button-cell';
    const commonCellClass = 'ag-grid-custom-common-cell';
    const centerCellClasses = [commonCellClass, 'ag-centered-cell'];
    const simpleHeaderClasses = ['ag-grid-custom-header-cell-accent-background', commonCellClass];
    const accentCellClasses = ['ag-grid-custom-accent-cell'];
    const columns: (ColDef | ColGroupDef)[] = [];
    contentMeta.filter(_ =>
      // ToDo: If need to exclude more columns, need to develope a proper way
      !_.name.toLocaleLowerCase().includes('organization')
    ).sort((a, b) => a.depth - b.depth).map(x => ({
      colId: `${x.depth}-content-tree`,
      headerName: `${capitalizeFirstLetter(x.name)}`,
      headerClass: simpleHeaderClasses,
      cellClass: [commonCellClass, ...this.getClasses(x.name)],
      valueGetter: getValue(x.depth, x.name),
      suppressMovable: true,
      autoHeight: true,
      wrapText: true,
      flex: getFlex(x.name, x.depth),
    })).forEach(x => columns.push(x));
    var maxIndex = Math.max(...contentMeta.map(x => x.depth));

    columns.push(...
      [
        {
          colId: `${maxIndex + 1}-assignedAt`,
          headerName: `${capitalizeFirstLetter(this.strings.dateAssigned)}`,
          headerClass: simpleHeaderClasses,
          cellClass: commonCellClass,
          valueGetter: (params: ValueGetterParams) => formatDate((params.node?.data as RowModel)?.assignedAt),
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          flex: 1.2,
        },
        {
          colId: `${maxIndex + 2}-dueAt`,
          headerName: `${capitalizeFirstLetter(this.strings.dueDate)}`,
          headerClass: simpleHeaderClasses,
          cellClass: commonCellClass,
          cellStyle: getDueDateCellColor,
          valueGetter: (params: ValueGetterParams) => formatDate((params.node?.data as RowModel)?.dueAt),
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          flex: 1.2,
        },
        {
          colId: `${maxIndex + 3}-${ACTION_COLUMN}`,
          headerName: '',
          headerClass: simpleHeaderClasses,
          cellClass: centerCellClasses,
          cellRenderer: 'buttonCellRenderer',
          cellRendererParams: {
            clicked: (data: any) => {
              // this.onResetPassword.bind(this)(data.studentId);
            },
            width: 120
          },
          valueGetter: this._getActionColumnText(),
          suppressMovable: true,
          autoHeight: true,
          wrapText: true,
          flex: 1.2,
        }
      ]);

    return columns.sort((a: ColDef, b: ColDef) => a?.colId?.localeCompare(b?.colId ?? '') ?? 0);
  }

  formatDate(date: Date | null) {
    return formatDate(date);
  }

  getClasses(name: string) {
    if (name.toLocaleLowerCase().includes("key knowledge")) return ['ag-grid-wraped-cell'];
    if (name.toLocaleLowerCase().includes("topic")) return ['ag-grid-wraped-cell'];
    if (name.toLocaleLowerCase().includes("heading")) return ['ag-grid-wraped-cell'];
    return [];
  }

  getCardActions(data: RowModel): ContentCardAction[] {
    var action = this._getActionText(data);
    return [{ name: action, id: "action" }];
  }

  private _getActionColumnText(): (params: ValueGetterParams) => string {
    return params => {
      const data = params.node?.data as RowModel | null | undefined;

      if (!data) {
        return '';
      }
      return this._getActionText(data);
    };
  }

  private _getActionText(data: RowModel) {
    if (!data) {
      return '';
    }

    if ('quizStudentAssignmentId' in data) {
      return capitalizeFirstLetter(this.strings.takeQuiz);
    }

    if ('learningMaterialUserAssignmentId' in data) {
      return capitalizeFirstLetter(this.strings.study);
    }

    throw Error('Either quizAssignmentId or learningMaterialUserAssignmentId must be specified for the row');
  }

  async onCardAction(action: ContentCardAction, data: RowModel) {
    if (action.id.includes('action')) {
      await this._handleAssignedWorkClick(data);
    }
  }

  getDueDateColor(date: Date | null): string {
    const local = toLocal(date);
    if (!local) return '';
    if (moment().isAfter(local.endOf('day')))
      return 'red';
    return 'green';
  }
}

function formatDate(date: Date | null | undefined): string {
  if (!date) return '—';

  return moment.utc(date).local().format('DD MMM YY');
}

function toLocal(date: Date | null | undefined): Moment | null {
  if (!date) return null;
  return moment.utc(date).local();
}

function convertAssignedWorkToRows(assignedWork: AssignedWork): RowModel[] {
  const result: RowModel[] = [];
  result.push(...assignedWork.quizzes);
  result.push(...assignedWork.learningMaterials);

  result.sort((x1, x2) => {
    const dueDate1 = toLocal(x1.dueAt)?.startOf('day');
    const dueDate2 = toLocal(x2.dueAt)?.startOf('day');

    if (dueDate1 && dueDate2 && dueDate1.valueOf() !== dueDate2.valueOf()) {
      return dueDate2.valueOf() < dueDate1.valueOf() ? 1 : -1;
    }
    if (!dueDate1 && dueDate2) return 1;
    if (dueDate1 && !dueDate2) return -1;

    const assignedDate1 = toLocal(x1.assignedAt)?.startOf('day');
    const assignedDate2 = toLocal(x2.assignedAt)?.startOf('day');
    if (assignedDate1 && assignedDate2 && assignedDate1.valueOf() !== assignedDate2.valueOf()) {
      return assignedDate1.valueOf() < assignedDate2.valueOf() ? 1 : -1;
    }

    return x1.nodeId.localeCompare(x2.nodeId);
  });

  return result;
}

export function getType(depth: number): string {
  if (depth != 2 && depth != 3)
    return 'centerAligned';

  return 'leftAligned';
}

function getValue(depth: number, columnName?: string): (params: ValueGetterParams) => string {
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

function tuneGrid(gridApi: GridApi): void {
  // gridApi.sizeColumnsToFit();
  gridApi.resetRowHeights();
}

export function getDueDateCellColor(params: ICellRendererParams): { 'color': string } | undefined {
  let date = toLocal((params.node?.data as RowModel)?.dueAt);
  if (!date) return undefined;
  if (moment().isAfter(date.endOf('day')))
    return { color: 'red' };
  return { color: 'green' };
}

export function getFlex(name: string, depth: number): number {
  let flex = 1;
  if (name.toLocaleLowerCase().includes("unit"))
    flex = 0.8;
  if (name.toLocaleLowerCase().includes("module"))
    flex = 0.8;
  if (name.toLocaleLowerCase().includes("content"))
    flex = 3;
  if (name.toLocaleLowerCase().includes("area"))
    flex = 1.2;
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
  if(name.toLocaleLowerCase().includes("organisation"))
    flex = 1.5;

  return flex;
}
