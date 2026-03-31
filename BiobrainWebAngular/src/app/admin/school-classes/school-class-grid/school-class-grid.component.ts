import { Component, EventEmitter, OnDestroy, OnInit, Output, SecurityContext } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { GridApi, GridOptions, ICellRendererParams } from 'ag-grid-community';
import { Subscription } from 'rxjs';
import { StringsService } from 'src/app/share/strings.service';

import { alphaNumericStringComparator } from '../../../share/helpers/alpha-numeric-comparer';

import { SchoolClassListStore } from './school-class-list-store';

@Component({
  selector: 'app-school-class-grid',
  templateUrl: './school-class-grid.component.html',
  styleUrls: ['./school-class-grid.component.scss']
})
export class SchoolClassGridComponent implements OnInit, OnDestroy {
  @Output() editSchoolClass = new EventEmitter<string>();
  @Output() deleteSchoolClass = new EventEmitter<string>();

  textFilterParams = {
    filterOptions: ['contains', 'notContains'],
    trimInput: true,
    debounceMs: 200,
  };

  numberFilterParams = {
    filterOptions: [
      'equals',
      'notEqual',
      'lessThan',
      'greaterThan',
      'lessThanOrEqual',
      'greaterThanOrEqual',
    ],
    suppressAndOrCondition: true,
  };

  private readonly _subscriptions: Subscription[] = [];
  private _gridApi: GridApi | null | undefined;

  constructor(
    public readonly schoolClassListStore: SchoolClassListStore,
    public readonly strings: StringsService,
    private readonly _sanitizer: DomSanitizer
  ) {
  }

  teachersRenderer = (params: ICellRendererParams): string =>
    (params.value as string[]).sort(alphaNumericStringComparator)
      .map(x => this._sanitizer.sanitize(SecurityContext.HTML,x))
      .join('</br>');

  ngOnInit(): void {
    this._subscriptions.push(
      this.schoolClassListStore.items$.subscribe(() => {
        this._gridApi?.sizeColumnsToFit();
        // this._gridApi?.resetRowHeights();
      }
      )
    );
  }

  ngOnDestroy(): void {
    this._subscriptions.forEach(s => s.unsubscribe());
  }

  onGridReady(params: GridOptions): void {
    this._gridApi = params.api;
    this._gridApi?.sizeColumnsToFit();
    // this._gridApi?.resetRowHeights();
  }

  onModelUpdated(): void {
    this._gridApi?.sizeColumnsToFit();
    this._gridApi?.onRowHeightChanged();
  }

  onEditSchoolClass(schoolClassId: string): void {
    this.editSchoolClass.next(schoolClassId);
  }

  onDeleteSchoolClass(schoolClassId: string): void {
    this.deleteSchoolClass.next(schoolClassId);
  }
}
