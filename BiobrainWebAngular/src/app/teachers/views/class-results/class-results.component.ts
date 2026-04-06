import { TitleCasePipe } from '@angular/common';
import { Component, ElementRef, HostListener, Input, OnInit, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridOptions, ValueGetterParams } from 'ag-grid-community';
import moment from 'moment';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { filter, map, tap, withLatestFrom } from 'rxjs/operators';
import {
  ClassResultsQuery_Result_QuizResultData,
  GetClassResultsQuery_Result,
} from 'src/app/api/quizzes/get-class-results.query';
import { studentByNameComparer } from 'src/app/api/quizzes/quiz-analytic-output.models';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import {
  NavigatableCellRendererComponent,
  NavigatableCellRendererProps,
} from 'src/app/share/components/renderers/navigatable-cell-renderer/navigatable-cell-renderer.component';
import { ProgressCycleRendererComponent } from 'src/app/share/components/renderers/progress-cycle-renderer/progress-cycle-renderer.component';
import { capitalizeFirstLetter } from 'src/app/share/helpers/capitalize-first-letter';
import { StringsService } from 'src/app/share/strings.service';

import { QuizNameHeaderRendererComponent } from '../../../share/components/renderers/quiz-name-header-renderer/quiz-name-header-renderer.component';
import { hasValue } from '../../../share/helpers/has-value';
import { getMaybeNotApplicableCellColor } from '../../helpers/get-maybe-not-applicable-cell-color';
import { getStudentAverageQuizScore } from '../../helpers/get-student-average-quiz-score';
import { getStudentNameGetter } from '../../helpers/get-student-name-getter';
import { getStudentProgress } from '../../helpers/get-student-progress';
import { parseStudentIdFromGetterParams } from '../../helpers/parse-student-id-from-getter-params';
import { DownloadClassResultsOperation } from '../../operations/download-class-results.operation';

import { ClassResultsStore, ClassResultsStoreItem, ClassResultsStoreItem_QuizAssignment, ClassResultsStoreItem_QuizStudentAssignment } from './class-results-store';
import { ContentTreeMetaFilterViewModel } from '../../view-models/content-tree-meta-filter.view-model';
import { ContentTreeService } from 'src/app/core/services/content/content-tree.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { ContentTreeMetaFilterValue } from '../../view-models/content-tree-meta-filter-value.view-model';

const BUFFER_SIZE = 1;

@Component({
  selector: 'app-class-results',
  templateUrl: './class-results.component.html',
  styleUrls: ['./class-results.component.scss'],
  providers: [ClassResultsStore],
})
export class ClassResultsComponent extends DisposableSubscriberComponent implements OnInit {
  private _cachedClassName = '';
  private _cachedCourseId = '';
  private _cachedSchoolClassId = '';
  private readonly _schoolClassId$ = new ReplaySubject<string | null | undefined>(BUFFER_SIZE);
  private readonly _courseId$ = new ReplaySubject<string | null | undefined>(BUFFER_SIZE);
  private readonly _filter$ = new ReplaySubject<string | null | undefined>(BUFFER_SIZE);
  private readonly _gridApi: ReplaySubject<GridApi> = new ReplaySubject<GridApi>(BUFFER_SIZE);
  private readonly _gridApi$: Observable<GridApi> = this._gridApi.asObservable();
  private readonly _sizeGridToFit$: ReplaySubject<undefined> = new ReplaySubject<undefined>(BUFFER_SIZE);
  filters: ContentTreeMetaFilterViewModel[] = [];
  anonymizeResults = false;
  private _anonymizedNameMap = new Map<string, string>();
  private _lastResults: ClassResultsStoreItem | null = null;

  constructor(
    public readonly strings: StringsService,
    public readonly store: ClassResultsStore,
    private readonly _downloadClassResultsOperation: DownloadClassResultsOperation,
    private readonly _titleCasePipe: TitleCasePipe,
    private readonly _routingService: RoutingService,
    private readonly _contentTree: ContentTreeService,
  ) {
    super();
    this._filter$.next("");
  }

  @ViewChild('agGridRef') agGrid?: AgGridAngular;
  @ViewChild('agGridRef', { read: ElementRef }) agGridElement?: ElementRef;

  @Input() set schoolClassId(value: string | null | undefined) {
    this._schoolClassId$.next(value);
  }

  @Input() set courseId(value: string | null | undefined) {
    this._courseId$.next(value);
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

  onModelUpdated(): void {
    this._sizeGridToFit$.next();
  }

  ngOnInit(): void {
    this.store.attachBinding(
      this._schoolClassId$.pipe(
        withLatestFrom(this._courseId$, this._filter$),
        filter(([schoolClassId, courseId, filter]) => Boolean(schoolClassId) && Boolean(courseId)),
        tap(([schoolClassId, courseId, filter]) => {
          this._cachedCourseId = courseId ?? '';
          this._cachedSchoolClassId = schoolClassId ?? '';

        }),
        tap(([schoolClassId, courseId, filter]) => {
          this.setupFilters(courseId);
        }),
        map(([schoolClassId, courseId, filter]) => ({
          schoolClassId: schoolClassId ?? '',
          courseId: courseId ?? '',
          selectedFilterNodes: this.filters.map(f => f.selectedValue?.nodeId ?? '').filter(_ => _.length > 0),
        })),

      ),
      
    );

    this.store.attachReload(
      this._filter$.pipe(
        withLatestFrom(this._schoolClassId$, this._courseId$),
        filter(([filter, schoolClassId, courseId]) => Boolean(schoolClassId) && Boolean(courseId)),
        // tap(_ => console.log(this.filters.map(f => f.selectedValue?.nodeId ?? '').filter(_ => _.length > 0))),
        map(([filter, schoolClassId, courseId]) => ({
          schoolClassId: schoolClassId ?? '',
          courseId: courseId ?? '',
          selectedFilterNodes: this.filters.map(f => f.selectedValue?.nodeId ?? '').filter(_ => _.length > 0),
        })),
        tap(_ => this.store.updateCriteria(_))
      ),
      
    );

    this.pushSubscribtions(
      this._gridApi.subscribe(x => x.sizeColumnsToFit()),

      combineLatest([this._sizeGridToFit$, this._gridApi$])
        .pipe(map(([_, gridApi]) => gridApi))
        .subscribe(x => {
          x.sizeColumnsToFit();
        }),

      combineLatest([this.store.item$, this._gridApi$]).subscribe(([results, gridApi]) => {
        this._lastResults = results;
        if (this.anonymizeResults && results) {
          this._buildAnonymizedNameMap();
        }
        gridApi.setColumnDefs(this._buildColumns(results));
        gridApi.setRowData(this._buildRows(results));
        this._sizeGridToFit$.next();
      }),
    );
  }

  async setupFilters(courseId: string | null | undefined) {
    if (!courseId) return;

    let meta = await firstValueFrom(this._contentTree.getMeta(courseId ?? ''));
    if (!meta) return;

    this.filters = [];
    for(let item of meta.sort((a,b) => a.dbRow.depth - b.dbRow.depth)){
      this.filters.push(new ContentTreeMetaFilterViewModel(item.dbRow.contentTreeMetaId, item.dbRow.name, item.dbRow.depth));
    }

    if(!this.filters[0]) return;
    this.filters[0].values = (await firstValueFrom(this._contentTree.getMetaValuesWithParent(this.filters[0].contentTreeMetaId, null)))?.map(_ => new ContentTreeMetaFilterValue(_.row.nodeId, _.row.name)) ?? [];
  }

  async filterSelected(contentTreeMetaId: string){
    var filter = this.filters.find(_ => _.contentTreeMetaId == contentTreeMetaId) ?? null;
    if(filter == null) return;

    this.filters.filter(_ => _.depth > (filter?.depth ?? 0)).forEach(_ => {_.selectedValue = null; _.values = [];})
    if(filter.selectedValue){
      var nextFilter = this.filters.find(_ => _.depth == (filter?.depth ?? -2)+1);
      if(nextFilter){
        nextFilter.values = (await firstValueFrom(this._contentTree.getMetaValuesWithParent(nextFilter.contentTreeMetaId, filter.selectedValue.nodeId)))?.map(_ => new ContentTreeMetaFilterValue(_.row.nodeId, _.row.name)) ?? [];
      }
    }

    this._filter$.next("a");
  }  
  
  async filterClicked(index: number){
    var selectElement = document.getElementById(`filter${index}`);
    console.log(selectElement)
    if(!selectElement) return;
    selectElement.click();
  }

  onAnonymizeToggle(): void {
    if (this.anonymizeResults) {
      this._buildAnonymizedNameMap();
    } else {
      this._anonymizedNameMap.clear();
    }
    this._gridApi.subscribe(gridApi => {
      gridApi.setColumnDefs(this._buildColumns(this._lastResults));
      gridApi.setRowData(this._buildRows(this._lastResults));
      gridApi.sizeColumnsToFit();
    });
  }

  private static readonly _FAKE_NAMES = [
    'Alex', 'Jordan', 'Riley', 'Casey', 'Morgan', 'Taylor', 'Quinn', 'Avery',
    'Dakota', 'Skyler', 'Reese', 'Finley', 'Hayden', 'Emerson', 'Parker',
    'Rowan', 'Sage', 'Blake', 'Charlie', 'Drew', 'Ellis', 'Frankie', 'Harper',
    'Indigo', 'Jamie', 'Kerry', 'Logan', 'Marley', 'Nico', 'Oakley', 'Peyton',
    'Robin', 'Sam', 'Tatum', 'Uri', 'Val', 'Winter', 'Xen', 'Yael', 'Zion',
  ];

  private _buildAnonymizedNameMap(): void {
    this._anonymizedNameMap.clear();
    if (!this._lastResults) return;

    const shuffledNames = [...ClassResultsComponent._FAKE_NAMES].sort(() => Math.random() - 0.5);
    const shuffledStudents = [...this._lastResults.students].sort(() => Math.random() - 0.5);
    shuffledStudents.forEach((student, index) => {
      const fakeName = index < shuffledNames.length
        ? shuffledNames[index]
        : `Student ${index + 1}`;
      this._anonymizedNameMap.set(student.studentId, fakeName);
    });
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this._gridApi.subscribe(x => x.sizeColumnsToFit());
  }

  async download() {
    if (!this._cachedCourseId || !this._cachedSchoolClassId) return;
    var result = await this._downloadClassResultsOperation.perform(this._cachedCourseId, this._cachedSchoolClassId);

    if (!result.isSuccess()) return;

    const anchor = document.createElement('a');
    anchor.download = `${this._cachedClassName}_${moment().format('YYYY_MM_DD')}.csv`;
    anchor.href = result.data;
    anchor.click();
  }

  private _buildRows(results: GetClassResultsQuery_Result | null): Array<{ studentId: string }> {
    if (results === null) {
      return [];
    }

    const rows = results.students.sort(studentByNameComparer).map(x => ({ studentId: x.studentId }));
    if (this.anonymizeResults) {
      return [...rows].sort(() => Math.random() - 0.5);
    }
    return rows;
  }

  private _buildColumns(results: ClassResultsStoreItem | null): ColDef[] {
    const aggregateCellClass = 'ag-grid-custom-aggregate-cell';
    const commonCellClass = 'ag-grid-custom-common-cell';
    const simpleHeaderClasses = ['ag-grid-custom-header-cell', commonCellClass];
    const aggregateHeaderClasses = ['ag-grid-custom-header-cell', aggregateCellClass];

    const minWidth = 150;

    if (results === null) {
      return [];
    }

    const {
      classData,
      subjectData,
      students,
      quizResults,
      quizStudentAssignments,
      progressInfo,
      averageScoreInfo,
    } = results;

    this._cachedClassName = classData.schoolClassName;
    var index = 0;
    return [
      {
        colId: 'studentName',
        headerName:
          `${capitalizeFirstLetter(this.strings.class)}  ${classData.schoolClassName ?? ''}`,
        // `${capitalizeFirstLetter(this.strings.year)} ${classData.schoolClassYear ?? ''} ${subjectData.subjectName ?? ''} `
        // + `${capitalizeFirstLetter(this.strings.class)}  ${classData.schoolClassName ?? ''} ${this._titleCasePipe.transform(this.strings.classResults)}`,
        headerClass: [...simpleHeaderClasses, 'ag-grid-custom-first-column-header'],
        cellClass: [aggregateCellClass, 'ag-grid-custom-first-column-cell'],
        valueGetter: this.anonymizeResults
          ? getAnonymizedStudentNameGetter(this._anonymizedNameMap)
          : getStudentNameGetter(students),
        minWidth: 240,
        suppressMovable: true,
        autoHeight: true,
        pinned: 'left',
        cellRendererFramework: NavigatableCellRendererComponent,
        cellRendererParams: {
          handleNavigate: async (params: NavigatableCellRendererProps) => {
            // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
            const studentId = params?.data.studentId as string;
            await this._routingService.navigateToStudentQuizResults(studentId);
          },
        },
      },
      {
        colId: 'avgScore',
        headerName: `${this._titleCasePipe.transform(this.strings.averageQuizScore)}`,
        headerClass: aggregateHeaderClasses,
        cellClass: aggregateCellClass,
        valueGetter: getStudentAverageQuizScore(averageScoreInfo, this.strings),
        minWidth: 100,
        width: 100,
        suppressMovable: true,
        autoHeight: true,
        wrapText: true,
        pinned: 'left',
        cellStyle: getMaybeNotApplicableCellColor,
      },
      {
        colId: 'progress',
        headerName: `${this._titleCasePipe.transform(this.strings.studentProgress)}`,
        headerClass: aggregateHeaderClasses,
        cellClass: aggregateCellClass,
        valueGetter: getStudentProgress(progressInfo),
        minWidth: 120,
        width: 120,
        suppressMovable: true,
        autoHeight: true,
        wrapText: true,
        pinned: 'left',
        cellRendererFramework: ProgressCycleRendererComponent,
      },
      ...results.quizAssignments.map(x => ({
        colId: x.quizAssignmentId,
        headerName: x.quizName,
        headerTooltip: x.quizName,
        headerClass: simpleHeaderClasses,
        cellClass: commonCellClass,
        valueGetter: getStudentQuizScore(quizResults, quizStudentAssignments, x, this.strings),
        minWidth,
        autoHeight: true,
        wrapText: true,
        suppressMovable: true,
        cellStyle: getMaybeNotApplicableCellColor,
        headerComponentFramework: QuizNameHeaderRendererComponent,
        headerComponentParams: {
          handleClick: async () => {
            await this._routingService.navigateToClassQuizAssignmentResult(
              x.quizAssignmentId,
            );
          },
          quizName: x.quizName,
          index: index++
        },
      })),
    ];
  }

  getStudentsCount(): Observable<number> {
    return this.store.item$.pipe(
      map(x => x?.students?.length ?? 0)
    );
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

    this.agGrid.api.setHeaderHeight(maxHeight + 40);
  }
}

function getStudentQuizScore(
  quizResults: ClassResultsQuery_Result_QuizResultData[],
  quizStudentAssignments: ClassResultsStoreItem_QuizStudentAssignment[],
  quizAssignment: ClassResultsStoreItem_QuizAssignment,
  strings: StringsService,
): (params: ValueGetterParams) => string {
  return params => {
    const studentId = parseStudentIdFromGetterParams(params);

    const studentAsignment = quizStudentAssignments.find(x => x.assignedToUserId === studentId && x.quizAssignmentId === quizAssignment.quizAssignmentId);

    const scoreInfo = quizResults.find(x => x.studentId === studentId && x.quizAssignmentId === quizAssignment.quizAssignmentId);
    if (!scoreInfo) {
      return hasValue(studentAsignment) ? strings.assignedShort : strings.notApplicableShort;
    }

    return scoreInfo.notApplicable || scoreInfo.score === null || scoreInfo.score === undefined
      ? hasValue(studentAsignment) ? strings.assignedShort : strings.notApplicableShort
      : `${Number(scoreInfo.score.toFixed(0)).toLocaleString()} %`;
  };
}

function getAnonymizedStudentNameGetter(
  nameMap: Map<string, string>,
): (params: ValueGetterParams) => string {
  return params => {
    const studentId = parseStudentIdFromGetterParams(params);
    return nameMap.get(studentId) ?? 'Student';
  };
}
