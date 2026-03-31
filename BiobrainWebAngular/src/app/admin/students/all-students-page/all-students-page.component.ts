import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDatepicker } from '@angular/material/datepicker';
import { GridApi, GridOptions, ValueFormatterParams } from 'ag-grid-community';
import moment, { Moment } from 'moment';
import { Api } from 'src/app/api/api.service';
import { GetStudentsListQuery, GetStudentsListQuery_Result } from 'src/app/api/students/get-students-list.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';
import { StringsService } from 'src/app/share/strings.service';
import { AgGridSettings } from 'src/app/share/values/ag-grid-settings';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { TitleCasePipe } from '@angular/common';
import { PermanentDeleteUserOperation } from '../../operations/permanent-delete-user.operation';


@Component({
  selector: 'app-all-students-page',
  templateUrl: './all-students-page.component.html',
  styleUrls: ['./all-students-page.component.scss'],
})
export class AllStudentsPageComponent extends BaseComponent implements OnInit {

  @ViewChild('fromPicker') fromDatePicker: MatDatepicker<moment.Moment> | undefined;
  @ViewChild('toPicker') toDatePicker: MatDatepicker<moment.Moment> | undefined;
  fromDate = new FormControl(moment("01.01.2000"));
  toDate = new FormControl(moment().endOf('day'));

  students: GetStudentsListQuery_Result[] = [];

  textFilterParams = AgGridSettings.textFilterParams;
  private _gridApi: GridApi | null | undefined;
  
  constructor(
    private readonly _api: Api,
    private readonly _loader: LoaderService,
    private readonly _permanentDeleteUserOperation: PermanentDeleteUserOperation,
    public readonly strings: StringsService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);
  }

  async ngOnInit() {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(this._titlecasePipe.transform(this.strings.students));
    }, 0);
    this.students = await this.getStudentsList();
  }

  onGridReady(params: GridOptions): void {
    this._gridApi = params.api;
    this._gridApi?.sizeColumnsToFit();
  }

  onModelUpdated(): void {
    this._gridApi?.sizeColumnsToFit();
  }

  fromDatepickerClick() {
    this.fromDatePicker?.open();
  }

  toDatepickerClick() {
    this.toDatePicker?.open();
  }

  async dateChanged(){
    this.students = await this.getStudentsList();
  }
  
  formatDate = (params: ValueFormatterParams): string => (params.value as Moment)?.format('DD/MM/YY') ?? '-';
  arrayToString = (params: ValueFormatterParams): string => (params.value as string[]).join(', ');  

  async onDeleteStudent(studentId: string, email: string) {
    await this._permanentDeleteUserOperation.perform(studentId, email);
    this.students = await this.getStudentsList();
  }
  
  private async getStudentsList(): Promise<GetStudentsListQuery_Result[]>{
    try{
      this._loader.show();
      const from = (<moment.Moment>this.fromDate.value).startOf('day').toJSON();
      const to = (<moment.Moment>this.toDate.value).endOf('day').toJSON();
      var result = await firstValueFrom(this._api.send(new GetStudentsListQuery(from, to)));
      return result;
    }
    catch(e: any){
      this.error(e.message);
    }
    finally{
      this._loader.hide();
    }
    
    return [];
  }
}
