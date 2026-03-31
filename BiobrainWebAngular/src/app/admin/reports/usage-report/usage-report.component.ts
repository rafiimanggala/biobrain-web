import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocomplete, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDatepicker } from '@angular/material/datepicker';
import { MatSelect } from '@angular/material/select';
import moment from 'moment';
import { Observable } from 'rxjs';
import { map, debounceTime, distinctUntilChanged, startWith } from 'rxjs/operators';
import { Api } from 'src/app/api/api.service';
import { GetUsageReports } from 'src/app/api/reports/get-usage-reports.query';
import { GetSchoolListItemsQuery, GetSchoolListItemsQuery_Result } from 'src/app/api/schools/get-school-list-items.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { StringsService } from 'src/app/share/strings.service';

@Component({
  selector: 'app-usage-report',
  templateUrl: './usage-report.component.html',
  styleUrls: ['./usage-report.component.scss'],
})
export class UsageReportComponent extends BaseComponent implements OnInit {
  schools: GetSchoolListItemsQuery_Result[] = [];
  schoolCtrl = new FormControl();
  schoolMultiFilterCtrl= new FormControl();
  filteredSchools: Observable<GetSchoolListItemsQuery_Result[]> | undefined;

  @ViewChild('multiSelect', { static: true }) multiSelect: MatSelect | undefined;

  // selectedSchools: GetSchoolListItemsQuery_Result[] = [];
  // selectedSchool: GetSchoolListItemsQuery_Result | undefined;

  @ViewChild('fromPicker') fromDatePicker: MatDatepicker<moment.Moment> | undefined;
  @ViewChild('toPicker') toDatePicker: MatDatepicker<moment.Moment> | undefined;
  fromDate = new FormControl(moment().startOf('day').subtract(21, 'day'));
  toDate = new FormControl(moment().endOf('day'));

  constructor(
    private readonly _api: Api,
    public readonly strings: StringsService,
    appEvents: AppEventProvider
  ) {
    super(appEvents);
  }

  async ngOnInit() {
    try {
      const result = await firstValueFrom(this._api.send(new GetSchoolListItemsQuery()));

      if (!result) return;
      this.schools = result;

      this.filteredSchools = this.schoolMultiFilterCtrl.valueChanges
        .pipe(
          startWith(''),
          map(x => {
            const name = typeof x === 'string' ? x : x?.name;
            return name ? this._filterSchool(name as string) : this.schools.slice();
          }
          )
        );
    }
    catch (e) {
      throw e;
    }
  }

  private _filterSchool(value: string): GetSchoolListItemsQuery_Result[] {
    const filterValue = value.toLowerCase();

    return this.schools.filter(school => school.name.toLowerCase().indexOf(filterValue) > -1);
  }

  // schoolSelected(event: MatAutocompleteSelectedEvent): void {
  //   this.selectedSchool = event.option.value;
  // }

  // addSelectedSchool() {
  //   if (!this.selectedSchool) return;
  //   if (this.selectedSchools.some(_ => _.schoolId == this.selectedSchool?.schoolId)) return;

  //   this.selectedSchools.push(this.selectedSchool);
  //   this.selectedSchool = undefined;
  //   this.schoolCtrl.setValue("");
  // }

  // deleteSchool(school: GetSchoolListItemsQuery_Result) {
  //   var index = this.selectedSchools.indexOf(school);
  //   if (index == -1) return;
  //   this.selectedSchools.splice(index, 1);
  // }

  async onGetReport() {
    try {
      var selectedSchools = this.schoolCtrl.value as GetSchoolListItemsQuery_Result[];
      if(selectedSchools.length < 1) {
        this.error("You must select at least one school");
        return;
      }

      // if(this.selectedSchools.length < 1 && this.selectedSchool) {
      //   this.addSelectedSchool();
      // }
      const timeZoneId = Intl.DateTimeFormat().resolvedOptions().timeZone;
      const from = (<moment.Moment>this.fromDate.value).startOf('day').toJSON();
      const to = (<moment.Moment>this.toDate.value).endOf('day').toJSON();
      const schoolIds = selectedSchools.map(_ => _.schoolId);
      const result = await firstValueFrom(this._api.send(new GetUsageReports(schoolIds, from, to, timeZoneId)));

      if (!result.fileUrl) return;
      const anchor = document.createElement('a');
      anchor.download = `Usage-Report_${selectedSchools.length > 1 ? selectedSchools.length : selectedSchools[0].name}_School${selectedSchools.length > 1 ? 's' : ''}_${moment().format('YYYY_MM_DD')}.pdf`;
      anchor.href = result.fileUrl;
      anchor.click();
    }
    catch (e) {
      throw e;
    }
  }

  fromDatepickerClick() {
    this.fromDatePicker?.open();
  }

  toDatepickerClick() {
    this.toDatePicker?.open();
  }

  displayFn(school: GetSchoolListItemsQuery_Result): string {
    return school && school.name ? school.name : '';
  }
}

