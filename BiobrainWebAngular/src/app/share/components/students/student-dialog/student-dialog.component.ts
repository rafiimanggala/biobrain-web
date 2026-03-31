import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/api/api.service';
import { GetCurriculaWithCountryRelationQuery, GetCurriculaWithCountryRelationQuery_Result } from 'src/app/api/curricula/get-curricula-with-country-relation.query';
import { SchoolClasslListItemsSingleSelectStore } from 'src/app/share/list-item-stores/school-class-list-items-single-select-store';
import { SchoolClassListItemsStore } from 'src/app/share/list-item-stores/school-class-list-items-store';
import { StringsService } from 'src/app/share/strings.service';
import { Countries, worldCountries } from 'src/app/share/values/countries';

import { DialogAction } from '../../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../../core/dialogs/dialog-component';

import { StudentDialogData } from './student-dialog-data';

@Component({
  selector: 'app-student-dialog',
  templateUrl: './student-dialog.component.html',
  styleUrls: ['./student-dialog.component.scss'],
  providers: [SchoolClassListItemsStore, SchoolClasslListItemsSingleSelectStore],
})
export class StudentDialog extends DialogComponent<StudentDialogData, StudentDialogData> {
  auStates = ["Australian Capital Territory", "Northern Territory", "NSW", "Queensland", "South Australia", "Tasmania", "Victoria", "Western Australia" ];
  filtredCountries: Countries[] = [];

  curricula: GetCurriculaWithCountryRelationQuery_Result[] = [];
  filteredCurricula: GetCurriculaWithCountryRelationQuery_Result[] = [];

  constructor(
    public readonly strings: StringsService,
    _api: Api,
    @Inject(MAT_DIALOG_DATA) public readonly data: StudentDialogData,
  ) {
    super(data);
    this.filtredCountries = worldCountries;
    this.subscriptions.push(_api.send(new GetCurriculaWithCountryRelationQuery()).subscribe(x => {this.curricula = x; this.filterCurricula();}));
  }

  onCountryChanged(){
    var filter = this.data.country.toLowerCase();
    this.filtredCountries = worldCountries.filter(x => x.name.toLowerCase().includes(filter));
    this.filterCurricula();
    if(this.data.country != "Australia") this.data.state = '';
  }

  onStateChanged(){this.filterCurricula();}

  filterCurricula(){
    this.filteredCurricula.length = 0;
    var country = this.data.country.toLowerCase();
    var state = this.data.state?.toLowerCase();

    this.curricula.forEach(c => {
      if(!c.availableCountries) {
        this.filteredCurricula.push(c);
        return;
      }

      var matchedCountry = c.availableCountries.find(x => x.name.toLowerCase() == country);
      if(matchedCountry){
        if(!matchedCountry.states || matchedCountry.states.some(x => x.toLocaleLowerCase() == state)){
          this.filteredCurricula.push(c);
          return;
        }
      }
    });

    if(!this.filteredCurricula.find(x => x.curriculumCode == this.data.curriculumCode))
      this.data.curriculumCode = undefined;
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) {
      return;
    }

    this.close(DialogAction.save, this.data);
  }
}
