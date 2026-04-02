import { TitleCasePipe } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ColDef, GridApi, GridOptions, ValueGetterParams } from 'ag-grid-community';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { filter, map, switchMap, tap } from 'rxjs/operators';
import {
  GetQuizAssignmentResultQuery_Result,
  GetQuizAssignmentResultQuery_Result_QuestionResult,
} from 'src/app/api/quizzes/get-quiz-assignment-result.query';
import { studentByNameComparer } from 'src/app/api/quizzes/quiz-analytic-output.models';
import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { BooleanAsCycleDiagramRendererComponent } from 'src/app/share/components/renderers/boolean-as-cycle-diagram-renderer/boolean-as-cycle-diagram-renderer.component';
import { ProgressCycleRendererComponent } from 'src/app/share/components/renderers/progress-cycle-renderer/progress-cycle-renderer.component';
import { capitalizeFirstLetter } from 'src/app/share/helpers/capitalize-first-letter';
import { StringsService } from 'src/app/share/strings.service';

import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { getMaybeNotApplicableCellColor } from '../../helpers/get-maybe-not-applicable-cell-color';
import { getStudentAverageQuizScore } from '../../helpers/get-student-average-quiz-score';
import { getStudentNameGetter } from '../../helpers/get-student-name-getter';
import { getStudentProgress } from '../../helpers/get-student-progress';
import { parseStudentIdFromGetterParams } from '../../helpers/parse-student-id-from-getter-params';

import { QuizAssignmentResultStore, QuizAssignmentResultStoreItem } from './quiz-assignment-result-store';
import { QuizNameHeaderRendererComponent } from 'src/app/share/components/renderers/quiz-name-header-renderer/quiz-name-header-renderer.component';
import { NavigatableCellRendererProps } from 'src/app/share/components/renderers/navigatable-cell-renderer/navigatable-cell-renderer.component';
import { QuestionTooltipRendererComponent } from 'src/app/share/components/question-tooltip-renderer/question-tooltip-renderer.component';
import { QuestionTextService } from 'src/app/share/services/question-text.service';
import { Question } from "src/app/api/content/content-data-models";

const BUFFER_SIZE = 1;

@Component({
  selector: 'app-quiz-assignment-result',
  templateUrl: './quiz-assignment-result.component.html',
  styleUrls: ['./quiz-assignment-result.component.scss'],
  providers: [QuizAssignmentResultStore],
})
export class QuizAssignmentResultComponent extends DisposableSubscriberComponent implements OnInit {
  @Output() reassignQuiz = new EventEmitter<{ quizId: string; studentIds: string[] }>();
  private _selectedStudentIds: string[] = [];
  anonymizeResults = false;
  private _anonymizedNameMap = new Map<string, string>();
  private _lastResults: QuizAssignmentResultStoreItem | null = null;

  private readonly _selectionChangedSubject = new ReplaySubject<undefined>(BUFFER_SIZE);
  private readonly _selectionChanged$ = this._selectionChangedSubject.asObservable();

  private readonly _quizAssignmentId$ = new ReplaySubject<string | null | undefined>(BUFFER_SIZE);
  private readonly _sizeGridToFit$: ReplaySubject<undefined> = new ReplaySubject<undefined>(BUFFER_SIZE);

  private readonly _gridApiSubject: ReplaySubject<GridApi> = new ReplaySubject<GridApi>(BUFFER_SIZE);
  private readonly _gridApi$: Observable<GridApi> = this._gridApiSubject.asObservable();

  constructor(
    public readonly strings: StringsService,
    public readonly store: QuizAssignmentResultStore,
    private readonly _questionTextService: QuestionTextService,
    private readonly _titleCasePipe: TitleCasePipe,
  ) {
    super();
  }

  @Input() set quizAssignmentId(value: string | null | undefined) {
    this._quizAssignmentId$.next(value);
  }

  onGridReady(params: GridOptions): void {
    if (!params.api) {
      throw new Error('params.api must be defined');
    }
    // NB! This is unsupported and may break at any time
    try {
      (params.api as any).context.beanWrappers.tooltipManager.beanInstance.MOUSEOVER_SHOW_TOOLTIP_TIMEOUT = 0;
      params.tooltipShowDelay = 0;
    } catch (e) {
      console.error(e);
    }
    params.tooltipShowDelay = 0;

    this._gridApiSubject.next(params.api);
  }

  onModelUpdated(): void {
    this._sizeGridToFit$.next();
  }

  ngOnInit(): void {
    this.store.attachBinding(
      this._quizAssignmentId$.pipe(
        filter(quizAssignmentId => Boolean(quizAssignmentId)),
        map(quizAssignmentId => ({ quizAssignmentId: quizAssignmentId ?? '' })),
      ),
    );

    let data = this.store.item$.pipe(
      switchMap(result => {return this.updateQuestionTexts(result)})
    );

    this.pushSubscribtions(
      this._gridApiSubject.subscribe(x => x.sizeColumnsToFit()),

      combineLatest([this._sizeGridToFit$, this._gridApi$])
        .pipe(map(([_, gridApi]) => gridApi))
        .subscribe(x => {
          x.sizeColumnsToFit();
        }),

      combineLatest([this._gridApi$, this._selectionChanged$])
        .pipe(map(([gridApi, _]) => gridApi))
        .subscribe(gridApi => {
          const selectedRows = gridApi.getSelectedRows() as Array<{ studentId: string }>;
          this._selectedStudentIds = selectedRows.map(x => x.studentId);
        }),

      combineLatest([data, this._gridApi$])
      .subscribe(([results, gridApi]) => {
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

  async updateQuestionTexts(result: QuizAssignmentResultStoreItem | null): Promise<QuizAssignmentResultStoreItem| null>{
    if(!result) return null;
    for(let i = 0; i < result.questions.length; i++){
      let q = result.questions[i];
      if(q == null) continue;
      q.text = await this._questionTextService.transformQuestionText({
        text: q.text,
        header: q.header,
        questionTypeCode: q.questionTypeCode,
        questionId: q.questionId,
        answers: [],
        feedBack: "",
        hint: "",
        questionTypeName: ""
      }) ?? '';
    }
    return result;
  }

  onAnonymizeToggle(): void {
    if (this.anonymizeResults) {
      this._buildAnonymizedNameMap();
    } else {
      this._anonymizedNameMap.clear();
    }
    this._gridApiSubject.subscribe(gridApi => {
      gridApi.setColumnDefs(this._buildColumns(this._lastResults));
      gridApi.setRowData(this._buildRows(this._lastResults));
      gridApi.sizeColumnsToFit();
    });
  }

  private _buildAnonymizedNameMap(): void {
    this._anonymizedNameMap.clear();
    if (!this._lastResults) return;

    const shuffled = [...this._lastResults.students].sort(() => Math.random() - 0.5);
    shuffled.forEach((student, index) => {
      this._anonymizedNameMap.set(student.studentId, `Student ${index + 1}`);
    });
  }

  async onEmailStudentClick(): Promise<void> {
    const item = await firstValueFrom(this.store.item$);
    if (!hasValue(item)) {
      return;
    }

    this.reassignQuiz.next({ quizId: item.quizId, studentIds: this._selectedStudentIds });
  }

  onSelectionChanged(): void {
    this._selectionChangedSubject.next();
  }

  private _buildColumns(result: QuizAssignmentResultStoreItem | null): ColDef[] {
    const aggregateCellClass = 'ag-grid-custom-aggregate-cell';
    const commonCellClass = 'ag-grid-custom-common-cell';
    const simpleHeaderClasses = ['ag-grid-custom-header-cell', commonCellClass];
    const aggregateHeaderClasses = ['ag-grid-custom-header-cell', aggregateCellClass];

    const minWidth = 100;

    if (result === null) {
      return [];
    }

    const {
      averageScoreInfo,
      students,
      quizName,
      subjectData,
      classData,
      progressInfo,
      questionResults,
      questions,
    } = result;

    //  `${capitalizeFirstLetter(this.strings.year)} ${classData?.schoolClassYear ?? ''} ${subjectData.subjectName} `
    //       + `${classData?.schoolClassName ?? ''} ${this._titleCasePipe.transform(quizName)}`
    return [
      {
        colId: 'studentName',
        headerName: quizName,
        headerClass: [...simpleHeaderClasses, 'ag-grid-custom-first-column-cell'],
        cellClass: [aggregateCellClass, 'ag-grid-custom-first-column-cell'],
        valueGetter: this.anonymizeResults
          ? getAnonymizedStudentNameGetter(this._anonymizedNameMap)
          : getStudentNameGetter(students),
        autoHeight: true,
        minWidth: 250,
        suppressMovable: true,
        headerComponentFramework: QuizNameHeaderRendererComponent,
        headerComponentParams: {
          handleClick: async () => {
            // await this._routingService.navigateToClassQuizAssignmentResult(
            //   x.quizAssignmentId,
            // );
          },
          quizName: quizName,
        },
      },
      {
        colId: 'avgScore',
        headerName: `${this._titleCasePipe.transform(this.strings.quizScore)}`,
        headerClass: aggregateHeaderClasses,
        cellClass: aggregateCellClass,
        valueGetter: getStudentAverageQuizScore(averageScoreInfo, this.strings),
        autoHeight: true,
        minWidth,
        suppressMovable: true,
        cellStyle: getMaybeNotApplicableCellColor,
      },
      {
        colId: 'progress',
        headerName: `${this._titleCasePipe.transform(this.strings.studentProgress)}`,
        headerClass: aggregateHeaderClasses,
        cellClass: aggregateCellClass,
        valueGetter: getStudentProgress(progressInfo),
        autoHeight: true,
        minWidth,
        suppressMovable: true,
        cellRendererFramework: ProgressCycleRendererComponent,
      },
      ...questions.map(x => ({
        colId: x.questionId,
        headerName: x.header,
        headerClass: [...simpleHeaderClasses, 'clickable'],
        autoHeight: true,
        minWidth,
        valueGetter: this._getIsQuestionCorrectGetter(questionResults, x.questionId),
        cellRendererFramework: BooleanAsCycleDiagramRendererComponent,
        headerTooltip: x.text,//.substring(0, 50)}...`,
        tooltipShowDelay: 0,
        tooltipComponent: QuestionTooltipRendererComponent
        
      })),
    ];
  }

  private _buildRows(result: GetQuizAssignmentResultQuery_Result | null): Array<{ studentId: string }> {
    if (result === null) {
      return [];
    }

    return result.students.sort(studentByNameComparer).map(x => ({ studentId: x.studentId }));
  }

  private _getIsQuestionCorrectGetter(
    questionResults: GetQuizAssignmentResultQuery_Result_QuestionResult[],
    questionId: string,
  ): (params: ValueGetterParams) => boolean | undefined {
    return params => {
      const studentId = parseStudentIdFromGetterParams(params);

      const questionData = questionResults.find(x => x.studentId === studentId && x.questionId === questionId);

      if (!questionData) {
        return undefined;
      }

      return questionData.isCorrect;
    };
  }
}

function getAnonymizedStudentNameGetter(
  nameMap: Map<string, string>,
): (params: ValueGetterParams) => string {
  return params => {
    const studentId = parseStudentIdFromGetterParams(params);
    return nameMap.get(studentId) ?? 'Student';
  };
}
