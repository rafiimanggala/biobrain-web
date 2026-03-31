import { DatePipe, TitleCasePipe } from '@angular/common';
import { ElementRef, HostListener, ViewChild } from '@angular/core';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import {
  CellClassParams,
  ColDef,
  GridApi,
  GridOptions,
  ICellRendererParams,
  RowClassParams,
  ValueGetterParams,
} from 'ag-grid-community';
import { combineLatest, Observable, ReplaySubject, Subscription } from 'rxjs';
import { filter, map, withLatestFrom } from 'rxjs/operators';
import { GetStudentQuizAssignmentResultsQuery_Result } from 'src/app/api/quizzes/get-student-quiz-assignment-results.query';
import { getStudentFullName } from 'src/app/api/quizzes/quiz-analytic-output.models';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { ButtonWithTooltipHeaderRendererComponent } from 'src/app/share/components/renderers/button-with-tooltip-header-renderer/button-with-tooltip-header-renderer.component';
import {
  CheckboxRendererComponent,
  CheckboxRendererProps,
} from 'src/app/share/components/renderers/checkbox-renderer/checkbox-renderer.component';
import { ProgressCycleRendererComponent } from 'src/app/share/components/renderers/progress-cycle-renderer/progress-cycle-renderer.component';
import { QuizNameCellRendererComponent } from 'src/app/share/components/renderers/quiz-name-cell-renderer/quiz-name-cell-renderer.component';
import { AppHintDialog } from 'src/app/share/dialogs/app-hint/app-hint.dialog';
import { capitalizeFirstLetter } from 'src/app/share/helpers/capitalize-first-letter';
import { StringsService } from 'src/app/share/strings.service';

import { distinct } from '../../../share/helpers/distinct-arrays';
import { toNonNullableWithError } from '../../../share/helpers/to-non-nullable';
import { getMaybeNotApplicableCellColor } from '../../helpers/get-maybe-not-applicable-cell-color';

import {
  StudentQuizAssignmentResultsStore,
  StudentQuizAssignmentResultsStoreItem,
  StudentQuizAssignmentResultsStoreItem_Result_Item,
} from './student-quiz-assignment-results-store';


const BUFFER_SIZE = 1;

@Component({
  selector: 'app-student-quiz-assignment-results',
  templateUrl: './student-quiz-assignment-results.component.html',
  styleUrls: ['./student-quiz-assignment-results.component.scss'],
  providers: [StudentQuizAssignmentResultsStore],
})
export class StudentQuizAssignmentResultsComponent extends DisposableSubscriberComponent implements OnInit {

  private readonly _baseHintShowCounterKey = 'HintStudentQuizAssignmentResults_';

  @ViewChild('agGridRef') agGrid?: AgGridAngular;
  @ViewChild('agGridRef', { read: ElementRef }) agGridElement?: ElementRef;

  private gridApiTempSubscription?: Subscription;
  @Output() reassignQuizzesAndMaterials = new EventEmitter<ReassignData>();
  private readonly _selectedMaterialAssignments: Map<string, string> = new Map<string, string>();
  private readonly _selectedQuizAssignments: Set<string> = new Set<string>();
  // rowClassRules = {
  //   'ag-row-selected': (params: RowClassParams): boolean => {
  //     // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
  //     const rowData = params.data as (RowData | null | undefined);
  //     if (!rowData) {
  //       return false;
  //     }

  //     return this._selectedQuizAssignments.has(rowData.quizAssignmentId) || this._selectedMaterialAssignments.has(rowData.quizAssignmentId);
  //   },
  // };
  private readonly _studentId$ = new ReplaySubject<string | null | undefined>(BUFFER_SIZE);
  private readonly _courseId$ = new ReplaySubject<string | null | undefined>(BUFFER_SIZE);
  private readonly _schoolClassId$ = new ReplaySubject<string | null | undefined>(BUFFER_SIZE);
  private readonly _sizeGridToFit$: ReplaySubject<undefined> = new ReplaySubject<undefined>(BUFFER_SIZE);

  private readonly _gridApiSubject: ReplaySubject<GridApi> = new ReplaySubject<GridApi>(BUFFER_SIZE);
  private readonly _gridApi$: Observable<GridApi> = this._gridApiSubject.asObservable();

  constructor(
    public readonly strings: StringsService,
    public readonly store: StudentQuizAssignmentResultsStore,
    private readonly _titleCasePipe: TitleCasePipe,
    private readonly _dialog: Dialog,
    private readonly _datePipe: DatePipe,
  ) {
    super();
  }

  @Input() set studentId(value: string | null | undefined) {
    this._studentId$.next(value);
  }

  @Input() set courseId(value: string | null | undefined) {
    this._courseId$.next(value);
  }

  @Input() set schoolClassId(value: string | null | undefined) {
    this._schoolClassId$.next(value);
  }  

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this.gridApiTempSubscription = this._gridApiSubject.subscribe(x => {
      x.sizeColumnsToFit();
      this.gridApiTempSubscription?.unsubscribe();
    });
  }

  onGridReady(params: GridOptions): void {
    if (!params.api) {
      throw new Error('params.api must be defined');
    }

    this._gridApiSubject.next(params.api);
  }

  onModelUpdated(): void {
    this._sizeGridToFit$.next();
  }

  ngOnInit(): void {
    this.store.attachBinding(
      this._studentId$.pipe(
        withLatestFrom(this._courseId$, this._schoolClassId$),
        filter(([studentId, courseId, schoolClassId]) => Boolean(studentId) && Boolean(courseId) && Boolean(schoolClassId)),
        map(([studentId, courseId, schoolClassId]) => ({
          studentId: studentId ?? '',
          schoolClassId: schoolClassId ?? '',
          courseId: courseId ?? '',
        })),
      ),
    );

    this.pushSubscribtions(
      this._gridApiSubject.subscribe(x => x.sizeColumnsToFit()),
      this._courseId$.subscribe(x => this.showHint(x)),

      combineLatest([this._sizeGridToFit$, this._gridApi$])
        .pipe(map(([_, gridApi]) => gridApi))
        .subscribe(x => {
          x.sizeColumnsToFit();
        }),

      combineLatest([this.store.item$, this._gridApi$]).subscribe(([results, gridApi]) => {
        gridApi.setColumnDefs(this._buildColumns(results, gridApi));
        gridApi.setRowData(this._buildRows(results));
        this._sizeGridToFit$.next();
      }),
    );
  }

  private showHint(couseId: string | null | undefined) {
    let count = 0;
    if (couseId) {
      let showCountString = localStorage.getItem(this._baseHintShowCounterKey /*+ couseId*/);
      if (showCountString && Number(showCountString)) {        
        count = Number(showCountString);
      }
      if(count < 3){
        count++;
        localStorage.setItem(this._baseHintShowCounterKey/* + couseId*/, count.toString());
      }
      else{
        return;
      }
    }
    this._dialog.show(AppHintDialog, { text: this.strings.studentResultEmailStudentExplanation, duration: 10000 });
  }

  private _buildRows(
    result: GetStudentQuizAssignmentResultsQuery_Result | null,
  ): RowData[] {
    if (result === null) {
      return [];
    }

    return result.results.map(x => ({
      materialId: x.quizAssignmentId, // TODO: link to a material
      quizAssignmentId: x.quizAssignmentId,
      quizStudentAssignmentId: x.quizStudentAssignmentId,
    }));
  }

  private _buildColumns(result: StudentQuizAssignmentResultsStoreItem | null, gridApi: GridApi): ColDef[] {
    const aggregateCellClass = 'ag-grid-custom-aggregate-cell';
    const commonCellClass = 'ag-grid-custom-common-cell';
    const simpleHeaderClasses = ['ag-grid-custom-header-cell', commonCellClass];
    const aggregateHeaderClasses = ['ag-grid-custom-header-cell', aggregateCellClass];

    const minWidth = 100;

    if (result === null) {
      return [];
    }

    const {
      classData,
      subjectData,
      studentInfo,
      results,
    } = result;

    const redrawRow = (params: ICellRendererParams): void => {
      const row = gridApi.getDisplayedRowAtIndex(params.rowIndex);
      if (row !== null) {
        gridApi.redrawRows({ rowNodes: [row] });
      }
    };

    const firstAssignment = results.length === 0 ? undefined : results[0].quizStudentAssignmentId;

    return [
      {
        colId: 'quizAssignmentName',
        headerName:
          `${getStudentFullName(studentInfo)} \n`
          //+ `${capitalizeFirstLetter(this.strings.year)} ${classData.schoolClassYear ?? ''} ${subjectData.subjectName ?? ''} `
          + `${capitalizeFirstLetter(this.strings.class)} ${classData.schoolClassName ?? ''}`,
        headerClass: [...simpleHeaderClasses, 'ag-grid-custom-first-column-header'],
        cellClass: [aggregateCellClass, 'ag-centered-cell', 'ag-grid-custom-first-column-cell'],
        valueGetter: this._getQuizStudentAssignmentNameGetter(results),
        minWidth: 330,
        suppressMovable: true,
        autoHeight: true,
        cellRendererFramework: QuizNameCellRendererComponent,
        cellRendererParams: {}
      },
      {
        colId: 'progress',
        headerName: `${this._titleCasePipe.transform(this.strings.studentProgress)}`,
        headerClass: aggregateHeaderClasses,
        cellClass: aggregateCellClass,
        valueGetter: this._getProgressGetter(results),
        minWidth,
        suppressMovable: true,
        cellRendererFramework: ProgressCycleRendererComponent,
      },
      {
        colId: 'quizScore',
        headerName: `${this._titleCasePipe.transform(this.strings.quizScore)}`,
        headerClass: aggregateHeaderClasses,
        cellClass: [aggregateCellClass, 'ag-centered-cell'],
        valueGetter: this._getQuizScoreGetter(results),
        minWidth,
        suppressMovable: true,
        cellStyle: getMaybeNotApplicableCellColor,
      },
      {
        colId: 'completedAt',
        headerName: `${this._titleCasePipe.transform(this.strings.dateCompleted)}`,
        headerClass: simpleHeaderClasses,
        cellClass: [commonCellClass, 'ag-centered-cell'],
        valueGetter: this._getCompletedAtGetter(results),
        minWidth,
        suppressMovable: true,
        // cellStyle: getMaybeNotApplicableCellColor,
      },
      {
        colId: 'reassignQuiz',
        headerName: `${this._titleCasePipe.transform(this.strings.reassignQuiz)}`,
        headerClass: simpleHeaderClasses,
        cellClass: [commonCellClass, 'ag-centered-cell'],
        minWidth,
        suppressMovable: true,
        valueGetter: this._getQuizAssignmentId(results),
        cellRendererFramework: CheckboxRendererComponent,
        cellRendererParams: {
          checkedStateEvaluator: (params: CheckboxRendererProps) => {
            // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
            const quizAssignmentId = params.getValue() as string;
            return this._selectedQuizAssignments.has(quizAssignmentId);
          },
          onChecked: (checked: boolean, params: CheckboxRendererProps) => {
            // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
            const quizAssignmentId = params.getValue() as string;
            if (checked) {
              this._selectedQuizAssignments.add(quizAssignmentId);
            } else {
              this._selectedQuizAssignments.delete(quizAssignmentId);
            }

            redrawRow(params);
          },
        },
      },
      {
        colId: 'reassignLearningMaterial',
        headerName: `${this._titleCasePipe.transform(this.strings.reassignLearningMaterial)}`,
        headerClass: simpleHeaderClasses,
        cellClass: [commonCellClass, 'ag-centered-cell'],
        minWidth,
        suppressMovable: true,
        valueGetter: this._getAssignment(results),
        cellRendererFramework: CheckboxRendererComponent,
        cellRendererParams: {
          checkedStateEvaluator: (params: CheckboxRendererProps) => {
            // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
            const materialId = params.getValue() as StudentQuizAssignmentResultsStoreItem_Result_Item;
            return this._selectedMaterialAssignments.has(materialId.quizAssignmentId);
          },
          onChecked: (checked: boolean, params: CheckboxRendererProps) => {
            // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
            const assignment = params.getValue() as StudentQuizAssignmentResultsStoreItem_Result_Item;
            if (checked) {
              this._selectedMaterialAssignments.set(assignment.quizAssignmentId, assignment.contentTreeNodeId);
            } else {
              this._selectedMaterialAssignments.delete(assignment.quizAssignmentId);
            }

            redrawRow(params);
          },
        },
      },
      // {
      //   colId: 'emailStudentExplanation',
      //   headerClass: simpleHeaderClasses,
      //   minWidth: 400,
      //   suppressMovable: true,
      //   headerName: this.strings.studentResultEmailStudentExplanation,
      //   valueGetter: this._getQuizStudentAssignmentId(results),
      //   rowSpan: () => results.length,
      //   valueFormatter: () => '',
      //   colSpan: () => 2,
      //   cellClassRules: { 'show-cell': (params: CellClassParams) => params.value === firstAssignment },
      // },
      {
        colId: 'emailStudentButton',
        headerClass: simpleHeaderClasses,
        minWidth: 100,
        suppressMovable: true,
        headerName: this._titleCasePipe.transform(this.strings.emailStudent),
        valueGetter: this._getQuizStudentAssignmentId(results),
        valueFormatter: () => '',
        rowSpan: () => results.length,
        cellClassRules: { 'show-cell': (params: CellClassParams) => params.value === firstAssignment },
        headerComponentFramework: ButtonWithTooltipHeaderRendererComponent,
        headerComponentParams: {
          // showTooltipOnInit: true,
          tooltip: this.strings.studentResultEmailStudentExplanation,
          handleClick: () => {
            const quizIds = Array.from(this._selectedQuizAssignments).map(
              quizAssignmentId => toNonNullableWithError(`QuizAssignment with id = ${quizAssignmentId} not found`)(results.find(x => x.quizAssignmentId === quizAssignmentId))
            ).map(_ => _.quizId);

            const reassignData = {
              quizIds: distinct(quizIds),
              quizAssignmentIds: Array.from(this._selectedQuizAssignments),
              materialIds: Array.from(this._selectedMaterialAssignments.values()),
            };
            this.reassignQuizzesAndMaterials.next(reassignData);
          },
        },
      },
    ];
  }

  private _getCompletedAtGetter(
    results: StudentQuizAssignmentResultsStoreItem_Result_Item[],
  ): (params: ValueGetterParams) => string {
    return params => {
      const assignment = this._parseQuizStudentAssignment(results, params);
      return this._datePipe.transform(assignment.completedAt, 'dd MMM YY') ?? '';
    };
  }

  private _getQuizAssignmentId(
    results: StudentQuizAssignmentResultsStoreItem_Result_Item[],
  ): (params: ValueGetterParams) => string {
    return params => {
      const assignment = this._parseQuizStudentAssignment(results, params);
      return assignment.quizAssignmentId;
    };
  }

  private _getAssignment(
    results: StudentQuizAssignmentResultsStoreItem_Result_Item[],
  ): (params: ValueGetterParams) => StudentQuizAssignmentResultsStoreItem_Result_Item {
    return params => {
      const assignment = this._parseQuizStudentAssignment(results, params);
      return assignment;
    };
  }

  private _getQuizStudentAssignmentId(
    results: StudentQuizAssignmentResultsStoreItem_Result_Item[],
  ): (params: ValueGetterParams) => string {
    return params => {
      const assignment = this._parseQuizStudentAssignment(results, params);
      return assignment.quizStudentAssignmentId;
    };
  }

  private _getQuizScoreGetter(
    results: StudentQuizAssignmentResultsStoreItem_Result_Item[],
  ): (params: ValueGetterParams) => string {
    return params => {
      const assignment = this._parseQuizStudentAssignment(results, params);
      return assignment.notApplicable
        ? this.strings.notApplicableShort
        : `${Number(assignment.score.toFixed(0)).toLocaleString()} %`;
    };
  }

  private _getProgressGetter(
    results: StudentQuizAssignmentResultsStoreItem_Result_Item[],
  ): (params: ValueGetterParams) => number {
    return params => {
      const assignment = this._parseQuizStudentAssignment(results, params);

      return assignment.notApplicable
        ? Number.NaN
        : Number(assignment.score);
    };
  }

  private _getQuizStudentAssignmentNameGetter(
    results: StudentQuizAssignmentResultsStoreItem_Result_Item[],
  ): (params: ValueGetterParams) => string {
    return params => {
      const assignment = this._parseQuizStudentAssignment(results, params);
      return assignment.quizNameHtml;
    };
  }

  private _parseQuizStudentAssignment(
    results: StudentQuizAssignmentResultsStoreItem_Result_Item[],
    params: ValueGetterParams,
  ): StudentQuizAssignmentResultsStoreItem_Result_Item {
    // eslint-disable-next-line @typescript-eslint/no-unsafe-assignment
    const data = params.node?.data as (RowData | null | undefined);
    if (!data) {
      throw new Error('params.node?.data is not defined');
    }

    const { quizStudentAssignmentId } = data;
    if (!quizStudentAssignmentId) {
      throw new Error('Unable to parse quizStudentAssignmentId property');
    }

    const assignment = results.find(x => x.quizStudentAssignmentId === quizStudentAssignmentId);
    if (!assignment) {
      throw new Error(`QuizStudentAssignment with id = ${quizStudentAssignmentId} not found`);
    }

    return assignment;
  }
}


interface RowData {
  readonly materialId: string;
  readonly quizAssignmentId: string;
  readonly quizStudentAssignmentId: string;
}


export interface ReassignData {
  readonly quizIds: string[];
  readonly quizAssignmentIds: string[];
  readonly materialIds: string[];
}
