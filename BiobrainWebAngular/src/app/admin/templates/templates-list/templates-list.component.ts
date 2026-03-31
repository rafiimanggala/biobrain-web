import { Component, HostListener, OnInit } from '@angular/core';
import { GridApi, GridOptions, ValueGetterParams } from 'ag-grid-community';
import { DisposableSubscriberComponent } from 'src/app/share/components/disposable-subscriber.component';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { StringsService } from 'src/app/share/strings.service';
import { SubTitleProviderService } from '../../services/sub-title-provider.service';
import { TitleCasePipe } from '@angular/common';
import { alphaNumericStringComparator } from 'src/app/share/helpers/alpha-numeric-comparer';
import { TakeTemplatesOperation } from '../operations/get-templates.operation';
import { TemplateViewModel } from '../template.view-model';
import { LoaderService } from 'src/app/share/services/loader.service';
import { TemplateTypes } from '../template-types.enum';
import { DeleteTemplateOperation } from '../operations/delete-template.operation';
import { CreateTemplateOperation } from '../operations/careate-template.operation';
import { EditTemplateOperation } from '../operations/edit-template.operation';

@Component({
  selector: 'app-templates-list',
  templateUrl: './templates-list.component.html',
  styleUrls: ['./templates-list.component.scss'],
  providers: [
    TakeTemplatesOperation,
    DeleteTemplateOperation,
    CreateTemplateOperation,
    EditTemplateOperation
  ],
})
export class TemplatesListComponent extends DisposableSubscriberComponent implements OnInit {
  readonly strComparator = alphaNumericStringComparator;

  templates: TemplateViewModel[] = [];

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

  private _gridApi: GridApi | null | undefined;

  constructor(
    public readonly strings: StringsService,
    private readonly _routingService: RoutingService,
    private readonly _subTitleProvider: SubTitleProviderService,
    private readonly _titlecasePipe: TitleCasePipe,
    private readonly _takeTemplatesOperation: TakeTemplatesOperation,
    private readonly _deleteTemplateOperation: DeleteTemplateOperation,
    private readonly _createTemplateOperation: CreateTemplateOperation,
    private readonly _editTemplateOperation: EditTemplateOperation,
    private readonly _loader: LoaderService
  ) {
    super();
  }

  async ngOnInit() {
    setTimeout(() => {
      this._subTitleProvider.subTitleSubject.next(`${this._titlecasePipe.transform(this.strings.templates)}`);
    }, 0);

    await this.get();
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

  async get() {
    try {
      this._loader.show();
      var result = await this._takeTemplatesOperation.perform();
      if (result.isSuccess()) {
        this.templates = result.data;
      }
    }
    catch (e: any) {
      console.log(e);
    }
    finally {
      this._loader.hide();
    }
  }

  async edit(id: string) {
    var template = this.templates.find(_ => _.templateId == id);
    if(!template) return;

    await this._editTemplateOperation.perform(template);
    await this.get();
    
  }

  async delete(id: string) {
    try {
      var template = this.templates.find(_ => _.templateId == id);
      if(!template) return;

      await this._deleteTemplateOperation.perform(template);
    }
    catch (e: any) {
      console.log(e);
    }

    await this.get();
  }

  async create() {
    await this._createTemplateOperation.perform();
    await this.get();
  }  

  typeValueGetter = ((params: ValueGetterParams): string => {
    let type = (params.node?.data as TemplateViewModel).templateType;
    return TemplateTypes[type];
  }).bind(this);

  courseValueGetter = ((params: ValueGetterParams): string => {
    let courses = (params.node?.data as TemplateViewModel).courses;
    return courses.map(_ => _.name).join(", ");
  }).bind(this);

  // statusValueGetter = ((params: ValueGetterParams): string => {
  //   let status = (params.node?.data as SchoolListModel_Result).status;
  //   switch(status){
  //     case SchoolStatus.Archive: return this?.strings?.archive ?? '';
  //     case SchoolStatus.FreeTrial: return this?.strings?.freeTrial ?? '';
  //     case SchoolStatus.LiveCustomer: return this?.strings?.liveCustomer ?? '';
  //     default: return "";
  //   }
  // }).bind(this);  
}
