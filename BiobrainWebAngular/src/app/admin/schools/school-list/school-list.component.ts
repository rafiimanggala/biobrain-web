import { Component, HostListener, OnInit } from '@angular/core';
import { GridApi, GridOptions, ICellRendererParams, ValueGetterParams } from 'ag-grid-community';

import { CreateSchoolService } from '../services/create-school.service';
import { DeleteSchoolService } from '../services/delete-school.service';
import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { SchoolListStore } from './school-list-store';
import { SchoolNameRouterLinkRendererComponent } from 'src/app/share/components/renderers/school-name-router-link-renderer/school-name-router-link-renderer.component';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { TitleCasePipe } from '@angular/common';
import { UpdateSchoolAdminsService } from '../services/update-school-admins.service';
import { UpdateSchoolDetailsService } from '../services/update-school-details.service';
import { UpdateSchoolLicensesService } from '../services/update-school-licenses.service';
import { alphaNumericStringComparator } from 'src/app/share/helpers/alpha-numeric-comparer';
import { combineLatest } from 'rxjs';
import { startWith } from 'rxjs/operators';
import { SchoolStatus } from 'src/app/api/enums/school-status.enum';
import { SchoolListModel_Result } from 'src/app/api/schools/get-school-list.query';
import moment from 'moment';
import {firstValueFrom} from "../../../share/helpers/first-value-from";

@Component({
  selector: 'app-school-list',
  templateUrl: './school-list.component.html',
  styleUrls: ['./school-list.component.scss'],
  providers: [
    SchoolListStore,
    CreateSchoolService,
    UpdateSchoolDetailsService,
    DeleteSchoolService,
    UpdateSchoolLicensesService,
    UpdateSchoolAdminsService,
  ],
})
export class SchoolListComponent extends DisposableSubscriberComponent implements OnInit {
  readonly strComparator = alphaNumericStringComparator;
  readonly nameRenderer = SchoolNameRouterLinkRendererComponent;


  nameFilterParams = {
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

  dateCellClassRules = {
    "ag-red-cell": function(params: ICellRendererParams) {
      if(!params.data.endDate) return;
      return params.data.endDate.isSameOrBefore(moment.utc()) && !params.data.endDate.local().isSame(moment(), 'day');
    },
    "ag-green-cell": function(params: any) {
      if(!params.data.endDate) return;
      return params.data.endDate.isAfter(moment.utc()) && !params.data.endDate.local().isSame(moment(), 'day');
    },
    "ag-orange-cell": function(params: any) {
      if(!params.data.endDate) return;
      return params.data.endDate.local().isSame(moment(), 'day');
    },
  }

  private _gridApi: GridApi | null | undefined;

  constructor(
    public readonly schoolListStore: SchoolListStore,
    public readonly strings: StringsService,
    private readonly _createSchoolService: CreateSchoolService,
    private readonly _updateSchoolDetailsService: UpdateSchoolDetailsService,
    private readonly _deleteSchoolService: DeleteSchoolService,
    private readonly _updateSchoolLicensesService: UpdateSchoolLicensesService,
    private readonly _updateSchoolAdminsService: UpdateSchoolAdminsService,
    private readonly _routingService: RoutingService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
  ) {
    super();
  }

  ngOnInit(): void {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(`${this._titlecasePipe.transform(this.strings.schools)}`);
    }, 0);

    this.schoolListStore.attachReload(
      combineLatest([
        this._createSchoolService.createdSchool$.pipe(startWith({})),
        this._deleteSchoolService.deletedSchool$.pipe(startWith({})),
        this._updateSchoolDetailsService.updatedSchool$.pipe(startWith({})),
        this._updateSchoolLicensesService.updatedSchool$.pipe(startWith({})),
        this._updateSchoolAdminsService.updatedSchool$.pipe(startWith({})),
      ])
    );

    this.schoolListStore.bind({});

    this.subscriptions.push(
      this.schoolListStore.items$.subscribe(() =>
        this._gridApi?.sizeColumnsToFit()
      )
    );
  }

  onGridReady(params: GridOptions): void {
    this._gridApi = params.api;
    this._gridApi?.sizeColumnsToFit();
  }

  onModelUpdated(): void {
    this._gridApi?.sizeColumnsToFit();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: any) {
    this._gridApi?.sizeColumnsToFit();
  }

  viewSchool(id: string): Promise<boolean> {
    return this._routingService.navigateToTeachersAdminPage(id);
  }

  editSchool(id: string): void {
    this._updateSchoolDetailsService.perform(id);
  }

  deleteSchool(id: string): void {
    this._deleteSchoolService.perform(id);
  }

  editSchoolAdmins(id: string): void {
    this._updateSchoolAdminsService.perform(id);
  }

  editSchoolLicenses(id: string): void {
    this._updateSchoolLicensesService.perform(id);
  }

  async createSchool(): Promise<boolean> {
    this._createSchoolService.perform();
    const { schoolId } = await firstValueFrom(this._createSchoolService.createdSchool$);
    return this._routingService.navigateToTeachersAdminPage(schoolId);
  }

  statusValueGetter = ((params: ValueGetterParams): string => {
    let status = (params.node?.data as SchoolListModel_Result).status;
    switch(status){
      case SchoolStatus.Archive: return this?.strings?.archive ?? '';
      case SchoolStatus.FreeTrial: return this?.strings?.freeTrial ?? '';
      case SchoolStatus.LiveCustomer: return this?.strings?.liveCustomer ?? '';
      default: return "";
    }
  }).bind(this);

  startDateValueGetter = ((params: ValueGetterParams): string => {
    let startDate = (params.node?.data as SchoolListModel_Result).startDate;
    return startDate ? startDate.local().format("DD-MMM-YYYY") : '-';
  }).bind(this);

  endDateValueGetter = ((params: ValueGetterParams): string => {
    let endDate = (params.node?.data as SchoolListModel_Result).endDate;
    return endDate ? endDate.local().format("DD-MMM-YYYY") : '-';
  }).bind(this);
}
