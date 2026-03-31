import { Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { Api } from 'src/app/api/api.service';
import { GetCurriculaWithCountryRelationQuery, GetCurriculaWithCountryRelationQuery_Result } from 'src/app/api/curricula/get-curricula-with-country-relation.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { AppSettings } from 'src/app/share/values/app-settings';
import { Countries, worldCountries } from 'src/app/share/values/countries';
import { StringsService } from '../../../share/strings.service';
import { SignUpDetailsData, SignUpDetailsSettings } from './sign-up-details-data';
import { CurriculumService } from 'src/app/share/services/curriculum.service';

@Component({
  selector: 'app-sign-up-details',
  templateUrl: './sign-up-details.component.html',
  styleUrls: ['./sign-up-details.component.scss']
})
export class SignUpDetailsComponent implements OnInit, OnDestroy {
  @Output() signUp = new EventEmitter<SignUpDetailsData>();
  settings: SignUpDetailsSettings = new SignUpDetailsSettings();

  subscriptions: Subscription[] = [];
  data: SignUpDetailsData;

  get isClassCodeVisible(): boolean { return this.settings?.isClassCodeVisible ?? false; }
  set isClassCodeVisible(value: boolean) {
    if (!this.settings) { this.settings = { isClassCodeVisible: value, isAccessCodeVisible: false, isVoucherVisible: false }; }
    else {
      this.settings.isClassCodeVisible = value;
    }
  }

  get isAccessCodeVisible(): boolean { return this.settings?.isAccessCodeVisible ?? false; }
  set isAccessCodeVisible(value: boolean) {
    if (!this.settings) {
      this.settings = { isClassCodeVisible: false, isAccessCodeVisible: value, isVoucherVisible: false };
    }
    else {
      this.settings.isAccessCodeVisible = value;
    }
  }

  get isVoucherVisible(): boolean { return this.settings?.isVoucherVisible ?? false; }
  set isVoucherVisible(value: boolean) {
    if (!this.settings) {
      this.settings = { isClassCodeVisible: false, isAccessCodeVisible: false, isVoucherVisible: value };
    }
    else {
      this.settings.isVoucherVisible = value;
    }
  }

  get isClassCodeAvailable(): boolean { return !this._isFreeTrial; }
  get isAccessCodeAvailable(): boolean { return !this._isFreeTrial; }
  get isVoucherAvailable(): boolean { return !this._isFreeTrial; }

  auStates = ["Australian Capital Territory", "Northern Territory", "NSW", "Queensland", "South Australia", "Tasmania", "Victoria", "Western Australia"];
  filtredCountries: Countries[] = [];

  curricula: GetCurriculaWithCountryRelationQuery_Result[] = [];
  filteredCurricula: GetCurriculaWithCountryRelationQuery_Result[] = [];

  private _isFreeTrial: boolean = false;

  constructor(
    public readonly strings: StringsService,
    private readonly _curriculumService: CurriculumService,
    private readonly _appEvents: AppEventProvider,
    private readonly _api: Api,
    activatedRoute: ActivatedRoute,
  ) {
    this.data = new SignUpDetailsData();
    this.filtredCountries = worldCountries;
    this._isFreeTrial = activatedRoute.snapshot.url[0].path == AppSettings.freeTrialPath;
  }

  ngOnInit(): void {
    this.subscriptions.push(this._api.send(new GetCurriculaWithCountryRelationQuery()).subscribe(x => this.curricula = x));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(x => x.unsubscribe());
  }

  onSubmit(signUpForm: NgForm): void {
    if (!signUpForm.valid) return;
    if (!this.validate()) return;
    if (!this.settings.isClassCodeVisible) this.data.classCode = '';
    if (!this.settings.isAccessCodeVisible) this.data.accessCode = '';
    if (!this.settings.isVoucherVisible) this.data.voucher = '';

    this.signUp.emit(this.data);
  }

  onCountryChanged() {
    var filter = this.data.country.toLowerCase();
    this.filtredCountries = worldCountries.filter(x => x.name.toLowerCase().includes(filter));
    this.filterCurricula();
    if (this.data.country != "Australia") this.data.state = '';
  }

  onStateChanged() { this.filterCurricula(); }

  filterCurricula() {
    this.filteredCurricula.length = 0;
    var country = this.data.country.toLowerCase();
    var state = this.data.state.toLowerCase();

    this.filteredCurricula = this._curriculumService.filterCurricula(country, state, this.curricula);

    if (!this.filteredCurricula.find(x => x.curriculumCode == this.data.curriculumCode))
      this.data.curriculumCode = undefined;
  }

  validate(): boolean {
    var result = true;
    if (!this.data.classCode && !worldCountries.find(x => x.name == this.data.country)) {
      result = false;
      this._appEvents.errorEmit(this.strings.countryNotSelected);
    }

    return result;
  }

  async isAccessCodeChange() {
    if (this.isAccessCodeVisible) {
      this.settings.isClassCodeVisible = false;
      this.settings.isVoucherVisible = false;
    }
  }

  async isClassCodeChange() {
    if (this.isClassCodeVisible) {
      this.settings.isAccessCodeVisible = false;
      this.settings.isVoucherVisible = false;
    }
  }

  async isVoucherChange() {
    if (this.isVoucherVisible) {
      this.settings.isClassCodeVisible = false;
      this.settings.isAccessCodeVisible = false;
    }
  }
}
