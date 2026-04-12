import { Component, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { CellClickedEvent, ColDef, ColGroupDef, GridApi, GridOptions, ICellRendererParams, ValueGetterParams } from 'ag-grid-community';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { switchMap, map, distinctUntilChanged, tap } from 'rxjs/operators';
import { ContentTreeMetaListStore } from 'src/app/admin/content/content-mapper/content-tree-meta-list-store';
import { GetContentTreeMetaListQuery_Result } from 'src/app/api/content/get-content-tree-meta-list.query';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { ActiveCourseService } from 'src/app/core/services/active-course.service';
import { HistoryQuizResult } from 'src/app/core/services/quiz-reslts/history-quiz-result';
import { QuizResultHistory } from 'src/app/core/services/quiz-reslts/quiz-result-history';
import { QuizResultHistoryService } from 'src/app/core/services/quiz-reslts/quiz-result-history.service';
import { LearningContentProviderService } from 'src/app/learning-content/services/learning-content-provider.service';
import { capitalizeFirstLetter } from 'src/app/share/helpers/capitalize-first-letter';
import { toNonNullableWithError } from 'src/app/share/helpers/to-non-nullable';
import { TakeQuizOperation } from 'src/app/learning-content/operations/take-quiz.operation';
import { StringsService } from 'src/app/share/strings.service';

import { ThemeService } from '../../../core/app/theme.service';

import { QuizResultHistoryPageService } from './services/quiz-result-history-page.service';
import { ButtonCellRenderer } from '../../components/button-cell-renderer/button-cell-renderer.component';
import { getScoreColor } from 'src/app/teachers/helpers/get-score-color';
import { ContentCardAction } from '../../components/content-card/content-card-action';

const BUFFER_SIZE = 1;

@Component({
  selector: 'app-quiz-result-history',
  templateUrl: './quiz-result-history.component.html',
  styleUrls: ['./quiz-result-history.component.scss'],
  providers: [ContentTreeMetaListStore],
})
export class QuizResultHistoryComponent extends BaseComponent implements OnInit, OnDestroy {

  components = {
    buttonCellRenderer: ButtonCellRenderer
  }
  actions = { study: "study", retake: "retake" }
  cardActions: ContentCardAction[] = []

  data$: Observable<QuizResultHistory>;
  contentTreeMeta$: Observable<GetContentTreeMetaListQuery_Result[]>;
  subjectName: string = '';
  courseId: string = '';
  path = '';
  primaryColor: Observable<string>;
  accentColor: Observable<string>;

  private readonly _gridApi: ReplaySubject<GridApi> = new ReplaySubject<GridApi>(BUFFER_SIZE);
  private readonly _gridApi$: Observable<GridApi> = this._gridApi.asObservable();
  private readonly _sizeGridToFit$: ReplaySubject<undefined> = new ReplaySubject<undefined>(BUFFER_SIZE);

  constructor(
    public strings: StringsService,
    public readonly quizResultHistoryPageService: QuizResultHistoryPageService,
    private readonly _routingService: RoutingService,
    private readonly _takeQuizOperation: TakeQuizOperation,
    private readonly _activeCourseService: ActiveCourseService,
    _quizResultHistoryService: QuizResultHistoryService,
    _contentService: LearningContentProviderService,
    _contentTreeMetaListStore: ContentTreeMetaListStore,
    appEvents: AppEventProvider,
    themeService: ThemeService
  ) {
    super(appEvents);

    this.cardActions = [{ name: this.strings.study, id: this.actions.study }, { name: this.strings.retakeQuiz, id: this.actions.retake }];

    // Get data
    this.data$ = _activeCourseService.courseIdChanges$.pipe(
      map(toNonNullableWithError(this.strings.errors.courseMustBeSelected)),
      switchMap(courseId => _quizResultHistoryService.observeQuizResult(courseId))
    );

    // Subscribe to subject changes
    this.pushSubscribtions(
      this._activeCourseService.courseChanges$.pipe(
        distinctUntilChanged(),
        tap(x => {
          this.subjectName = x?.subject?.name ?? '';
          this.courseId = x?.courseId ?? '';
        }),
      ).subscribe()
    );

    // Get content tree meta
    this.contentTreeMeta$ = _activeCourseService.courseIdChanges$.pipe(
      map(toNonNullableWithError(this.strings.errors.courseMustBeSelected)),
      switchMap(courseId => {
        _contentTreeMetaListStore.bind({ courseId });
        return _contentTreeMetaListStore.items$;
      })
    );

    this.primaryColor = themeService.colors$.pipe(map(x => x.primary));
    this.accentColor = themeService.colors$.pipe(map(x => x.accent));
  }

  ngOnInit() {

    // Ini grid
    this.pushSubscribtions(

      this._gridApi.subscribe(x => x.sizeColumnsToFit()),

      combineLatest([this._sizeGridToFit$, this._gridApi$])
        .pipe(map(([_, gridApi]) => gridApi))
        .subscribe(x => {
          // x.sizeColumnsToFit();
        }),

      combineLatest([this.data$, this._gridApi$, this.contentTreeMeta$, this._activeCourseService.courseIdChanges$.pipe(map(toNonNullableWithError(this.strings.errors.courseMustBeSelected)))]).subscribe(([data, gridApi, contentTreeMeta, courseId]) => {
        gridApi.setColumnDefs(this._buildColumns(data.quizResults, contentTreeMeta));
        gridApi.setRowData(this._buildRows(data.quizResults));
        this._sizeGridToFit$.next();
      }),
    );
  }

  private _buildRows(quizResults: HistoryQuizResult[]): any[] {
    if (!quizResults) {
      return [];
    }

    return quizResults;
  }

  private _buildColumns(quizResults: HistoryQuizResult[], contentTreeMeta: GetContentTreeMetaListQuery_Result[]): (ColDef | ColGroupDef)[] {
    const buttonCellClass = 'ag-grid-custom-button-cell';
    const commonCellClass = 'ag-grid-custom-common-cell';
    const simpleHeaderClasses = ['ag-grid-custom-header-cell-accent-background', commonCellClass];
    const centerCellClasses = [commonCellClass, 'ag-centered-cell'];

    if (!quizResults || !contentTreeMeta) {
      return [];
    }

    const columnDefs: (ColDef | ColGroupDef)[] = [];

    contentTreeMeta.sort((a, b) => a.depth - b.depth).forEach(x => {
      // ToDo: If need to exclude more columns, need to develope a proper way
      if(!x.name.toLocaleLowerCase().includes("organization"))
        columnDefs.push(
          {
            colId: `${x.depth}-content-tree`,
            headerName: `${capitalizeFirstLetter(x.name)}`,
            headerClass: simpleHeaderClasses,
            cellClass: [commonCellClass, ...this.getClasses(x.name)],
            valueGetter: getContentTreeLevelNameGetter(x.depth),
            suppressMovable: true,
            autoHeight: true,
            flex: getFlex(x.name, x.depth),
            type: getType(x.depth)
          }
        );
    });
    var maxIndex = Math.max(...contentTreeMeta.map(x => x.depth));

    columnDefs.push(
      {
        colId: `${maxIndex + 1}-score`,
        headerName: `${capitalizeFirstLetter(this.strings.score)}`,
        headerClass: simpleHeaderClasses,
        cellClass: commonCellClass,
        valueGetter: getScoreGetter(),
        suppressMovable: true,
        autoHeight: true,
        minWidth: 80,
        flex: 1,
        cellStyle: getScoreCellColor
      }
    );
    columnDefs.push(
      {
        colId: `${maxIndex + 2}-date`,
        headerName: `${capitalizeFirstLetter(this.strings.date)}`,
        headerClass: simpleHeaderClasses,
        cellClass: commonCellClass,
        field: 'date',
        suppressMovable: true,
        autoHeight: true,
        minWidth: 100,
        flex: 1.5,
      }
    );
    columnDefs.push(
      {
        colId: `${maxIndex + 3}-study`,
        headerName: '',
        headerClass: simpleHeaderClasses,
        cellClass: centerCellClasses,
        cellRenderer: 'buttonCellRenderer',
        cellRendererParams: {
          clicked: (data: any) => {
            // this.onResetPassword.bind(this)(data.studentId);
          }
        },
        // cellClass: buttonCellClass,
        valueGetter: (() => this.strings.study).bind(this),
        suppressMovable: true,
        autoHeight: true,
        width: 90,
        type: 'centerAligned'
      }
    );
    columnDefs.push(
      {
        colId: `${maxIndex + 4}-retake`,
        headerName: '',
        headerClass: simpleHeaderClasses,
        cellClass: centerCellClasses,
        cellRenderer: 'buttonCellRenderer',
        cellRendererParams: {
          clicked: (data: any) => {
            // this.onResetPassword.bind(this)(data.studentId);
          }
        },
        // cellClass: buttonCellClass,
        valueGetter: (() => this.strings.retakeQuiz).bind(this),
        suppressMovable: true,
        autoHeight: true,
        width: 120,
        type: 'centerAligned'
      }
    );

    return columnDefs.sort((a: ColDef, b: ColDef) => a?.colId?.localeCompare(b?.colId ?? '') ?? 0);
  }

  getClasses(name: string) {
    if (name.toLocaleLowerCase().includes("key knowledge"))
      return ['ag-grid-wraped-cell'];
    if (name.toLocaleLowerCase().includes("topic"))
      return ['ag-grid-wraped-cell'];
    return [];
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

  async onCellClicked(params: CellClickedEvent) {
    const data = <HistoryQuizResult>params.node.data;
    if (!data) return;

    let id = params.column.getId();

    if (id.includes(this.actions.study)) {
      await this._routingService.navigateToMaterialPage(data.courseId, data.parentNodeId ?? undefined, undefined);
    }
    if (id.includes(this.actions.retake)) {
      await this._takeQuizOperation.perform(data.courseId, data.nodeId);
    }
  }

  async onCardAction(action: ContentCardAction, data: HistoryQuizResult) {

    if (action.id.includes(this.actions.study)) {
      await this._routingService.navigateToMaterialPage(data.courseId, data.parentNodeId ?? undefined, undefined);
    }
    if (action.id.includes(this.actions.retake)) {
      await this._takeQuizOperation.perform(data.courseId, data.nodeId);
    }
  }

  onCourseClick() {
    if (!this.courseId) return;
    this._routingService.navigateToMaterialPage(this.courseId, undefined, undefined);
  }
}

export function getContentTreeLevelNameGetter(depth: number): (params: ValueGetterParams) => string {
  return params => {
    const data = <HistoryQuizResult>params.node?.data;
    if (!data || !data.path) return '';

    return data.path[depth];
  };
}

export function getFlex(name: string, depth: number): number {
  let flex = 1;
  if (name.toLocaleLowerCase().includes("unit"))
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
    if (depth === 0)
      flex = 0.5;
    else
      flex = 2;
  }
  if(name.toLocaleLowerCase().includes("organisation"))
    flex = 1.5;

  return flex;
}

export function getType(depth: number): string {
  if (depth != 2 && depth != 3)
    return 'centerAligned';

  return 'leftAligned';
}

export function getScoreGetter(): (params: ValueGetterParams) => string {
  return params => {
    const data = <HistoryQuizResult>params.node?.data;
    if (!data || !data.path) return '';

    return `${data.score * 10}%`;
  };
}

export function getScoreCellColor(params: ICellRendererParams): { 'color': string } | undefined {
  const value = params.value as string;
  const progress = Number.parseFloat(value.replace('%', ''));
  if (Number.isNaN(progress)) return undefined;

  return getScoreStyle(progress);
}


export function getScoreStyle(progress: number): { 'color': string } | undefined {
  return {
    color: getScoreColor(progress)
  };
}
