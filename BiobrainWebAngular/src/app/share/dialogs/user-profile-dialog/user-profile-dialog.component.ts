import { Component, Inject, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Api } from 'src/app/api/api.service';
import { GetCurriculaWithCountryRelationQuery, GetCurriculaWithCountryRelationQuery_Result } from 'src/app/api/curricula/get-curricula-with-country-relation.query';

import { DialogAction } from '../../../core/dialogs/dialog-action';
import { DialogComponent } from '../../../core/dialogs/dialog-component';
import { StringsService } from '../../strings.service';
import { Countries, worldCountries } from '../../values/countries';

import { UserProfileDialogData } from './user-profile-dialog-data';
import { UserProfileDialogResult } from './user-profile-dialog-result';
import { CurriculumService } from '../../services/curriculum.service';

@Component({
  selector: 'app-user-profile-dialog',
  templateUrl: './user-profile-dialog.component.html',
  styleUrls: ['./user-profile-dialog.component.scss'],
})
export class UserProfileDialogComponent extends DialogComponent<UserProfileDialogData, UserProfileDialogResult> implements OnInit {

  auStates = ["Australian Capital Territory", "Northern Territory", "NSW", "Queensland", "South Australia", "Tasmania", "Victoria", "Western Australia" ];
  filtredCountries: Countries[] = [];

  curricula: GetCurriculaWithCountryRelationQuery_Result[] = [];
  filteredCurricula: GetCurriculaWithCountryRelationQuery_Result[] = [];

  constructor(
    public readonly strings: StringsService,
    private readonly _curriculumService: CurriculumService,
    private readonly _api: Api,
    @Inject(MAT_DIALOG_DATA) public readonly data: UserProfileDialogData,
  ) {
    super(data);
    this.filtredCountries = worldCountries;
  }

  ngOnInit(): void {
    this.subscriptions.push(this._api.send(new GetCurriculaWithCountryRelationQuery()).subscribe(x => {
      this.curricula = x;
      this.filterCurricula();
    }));
  }

  onClose(): void {
    this.close();
  }

  onSubmit(form: NgForm): void {
    if (!form.valid) return;

    this.close(DialogAction.save, new UserProfileDialogResult(this.data.firstName, this.data.lastName, this.data.country, this.data.state, this.data.curriculumCode));
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
    var state = this.data.state.toLowerCase();

    this.filteredCurricula = this._curriculumService.filterCurricula(country, state, this.curricula);

    if (!this.filteredCurricula.find(x => x.curriculumCode == this.data.curriculumCode))
      this.data.curriculumCode = undefined;
  }
}
