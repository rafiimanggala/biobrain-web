import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatCheckbox } from '@angular/material/checkbox';
import moment from 'moment';
import { PaymentPeriod } from 'src/app/api/enums/payment-period.enum';
import { GetSubscriptionParametersQuery_Result, GetSubscriptionParametersQuery_ResultCurriculum, GetSubscriptionParametersQuery_ResultPrice, GetSubscriptionParametersQuery_ResultSubject } from 'src/app/api/payments/get-subscription-parameters.query';
import { CurrentUser } from 'src/app/auth/services/current-user';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { PaymentStringsService } from 'src/app/payment/services/payment-strings.service';
import { hasValue } from 'src/app/share/helpers/has-value';
import { appCountries } from 'src/app/share/values/countries';
import { StringsService } from '../../../../share/strings.service';
import { FreeTrialData } from './free-trial.data';
import { SubscriptionData } from './subscription.data';

export enum SubscriptionDetailsMode {
  Registeration = 1,
  MyAccount = 2
}

@Component({
  selector: 'app-subscription-details',
  templateUrl: './subscription-details.component.html',
  styleUrls: ['./subscription-details.component.scss'],
})
export class SubscriptionDetailsComponent extends BaseComponent {
  @Input() isPaymentDetailsVisible: boolean = true;
  @Output() next = new EventEmitter<SubscriptionData>();
  @Output() useVoucher = new EventEmitter<SubscriptionData>();
  @Output() freeTrial = new EventEmitter<FreeTrialData>();
  @Output() editBillingDetails = new EventEmitter();
  @Output() cancelSubscription = new EventEmitter();
  @Output() cancel = new EventEmitter();
  private _subscriptionParameters: GetSubscriptionParametersQuery_Result | undefined;

  get isVoucherMode():boolean { return (this._subscriptionParameters?.voucherAmount ?? 0) > 0; }
  get subscriptionParameters(): GetSubscriptionParametersQuery_Result | undefined { return this._subscriptionParameters; }
  @Input() set subscriptionParameters(val: GetSubscriptionParametersQuery_Result | undefined) {
    this._subscriptionParameters = val;
    if (!val) return;
    // this.subjects = val.subjects.sort((a,b)=> a.subjectCode - b.subjectCode).map(x => { return { model: x, isSelected: x.isSelected, year: x.year  }; });
    this.sets = [];
    new Set(val.prices.filter(x => x.isDisplayed).map(x => x.subjectsNumber)).forEach(x => this.sets.push({ subjects: x, prices: val.prices.filter(y => y.subjectsNumber == x).sort((a, b) => a.period - b.period).map(_ => { return { model: _, isSelected: false }; }) }));
    this.sets = this.sets.sort((a, b) => a.subjects - b.subjects);
  }
  @Input() mode: SubscriptionDetailsMode = SubscriptionDetailsMode.Registeration;

  get selectedCurriculum(): GetSubscriptionParametersQuery_ResultCurriculum | undefined { return this.subscriptionParameters?.curricula.find(x => x.curriculumCode == (this.subscriptionParameters?.userCurriculumCode ?? -1)) }

  // subjects: { model: GetSubscriptionParametersQuery_ResultSubject, isSelected: boolean, year: number | undefined }[] = [];
  sets: { subjects: number, prices: { model: GetSubscriptionParametersQuery_ResultPrice, isSelected: boolean }[] }[] = [];

  user?: CurrentUser;

  periods: PaymentPeriod[] = [PaymentPeriod.Monthly, PaymentPeriod.Yearly];
  // selectedPeriod?: PaymentPeriod;

  get subjectsNumber(): number {
    var subjects = this.subscriptionParameters?.subjects.filter(x => x.isSelected).length ?? 0;
    var additionalSubjects = this.subscriptionParameters?.additionalSubjects.filter(x => x.isSelected).length ?? 0;
    // Additional subjects: 2 for price of 1
    let count = subjects + Math.round(additionalSubjects / 2);
    return count;
  }

  get totalString(): string {
    let total = this.total;
    return `${total ? (this._subscriptionParameters?.currency ?? '') + total : ''}`;
  }

  get total(): number | undefined {
    return this._subscriptionParameters?.prices.filter(_ => _.subjectsNumber == this.subjectsNumber)?.find(_ => _.period == this._subscriptionParameters?.selectedPaymentPeriod)?.value;
  }

  // get needYearSelection(): boolean { console.log(this.selectedCurriculum); return (this.selectedCurriculum?.years?.length ?? 0) > 0; }
  get isSubjectSelected(): boolean {
    // return this.needYearSelection
    //   ? ((this.subscriptionParameters?.subjects.some(x => x.isSelected) ?? false) || (this.subscriptionParameters?.additionalSubjects.some(x => x.isSelected) ?? false)) && (this.subscriptionParameters?.subjects.filter(x => x.isSelected).every(x => hasValue(x.year)) ?? false)
    //   : (this.subscriptionParameters?.subjects.some(x => x.isSelected) ?? false) || (this.subscriptionParameters?.additionalSubjects.some(x => x.isSelected) ?? false)

    return ((this.subscriptionParameters?.subjects.some(x => x.isSelected) ?? false) || (this.subscriptionParameters?.additionalSubjects.some(x => x.isSelected) ?? false))
      && (
        // For subjects with year selection - need to select year
        (this.subscriptionParameters?.subjects.filter(x => x.isSelected && x.isNeedYearSelection).every(x => hasValue(x.selectedProduct)) ?? false)
        // If no subjects with year selection - not need to check year
        || ((this.subscriptionParameters?.subjects.filter(x => x.isSelected && x.isNeedYearSelection).length ?? 0) < 1)
      );
  }

  get chooseSubjects(): string {
    return this.strings.chooseSubjects;
    // if (!this._subscriptionParameters) return this.strings.chooseSubjects;
    // switch (this._subscriptionParameters.country) {
    //   case appCountries.Australia: return this.strings.chooseSubjects;
    //   case appCountries.USA: return `${this.strings.chooseSubjects} (${this.strings.grade11_12})`;
    //   default: return `${this.strings.chooseSubjects} (${this.strings.year11_12})`;
    // }
  }

  get additionalSubjects(): string {
    if (!this._subscriptionParameters) return this.strings.additionalSubjects;
    switch (this._subscriptionParameters.country) {
      case appCountries.Australia: return `${this.strings.additionalSubjects} (${this.strings.years9_10})`;
      case appCountries.USA: return `${this.strings.additionalSubjects} (${this.strings.grade9_10})`;
      default: return `${this.strings.additionalSubjects} (${this.strings.year9_10})`;
    }
  }

  get offerIamge(): string {
    if (!this._subscriptionParameters) return 'signup_image_2.svg';
    switch (this._subscriptionParameters.country) {
      case appCountries.Australia: return 'signup_image_2.svg';
      case appCountries.USA: return 'signup_image_2_usa.svg';
      default: return 'signup_image_2.svg';
    }
  }

  constructor(
    public readonly strings: StringsService,
    public paymentStringsService: PaymentStringsService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);
  }

  getPriceText(price: GetSubscriptionParametersQuery_ResultPrice): string {
    return (this.subscriptionParameters ? this.subscriptionParameters?.currency : "$") + price.value + " " + (price.period == 1 ? this.strings.monthly : this.strings.annually);
  }

  getTotal(): string {
    return `${this.strings.yourTotal}${this.totalString}`;
  }

  getVoucher(): string {
    return `${this.strings.voucher}: $${this._subscriptionParameters?.voucherAmount}`;
  }

  getFooterMessage(): string {
    const showTaxesWarning = this.subscriptionParameters?.country === appCountries.Australia;
    if (this.isPaymentDetailsVisible) {
      switch (this.mode) {
        case SubscriptionDetailsMode.Registeration:
          return this.strings.subscriptionMessage(
            this.totalString ? this.totalString + ' ' : '',
            this.paymentStringsService.date,
            this.paymentStringsService.getNounPeriodText(this._subscriptionParameters?.selectedPaymentPeriod ?? PaymentPeriod.Monthly),
            this.paymentStringsService.getPeriodText(this._subscriptionParameters?.selectedPaymentPeriod ?? PaymentPeriod.Monthly),
            showTaxesWarning
          );
        case SubscriptionDetailsMode.MyAccount:
          return this.strings.myAccountSubscriptionMessage(
            this.totalString ? this.totalString + ' ' : '',
            this.paymentStringsService.date,
            this.paymentStringsService.getNounPeriodText(this._subscriptionParameters?.selectedPaymentPeriod ?? PaymentPeriod.Monthly),
            this.paymentStringsService.getPeriodText(this._subscriptionParameters?.selectedPaymentPeriod ?? PaymentPeriod.Monthly),
            showTaxesWarning
          );
      }
    } else {
      return this.strings.freeTrialSubscriptionMessage(moment().add(7, 'days').format("DD/MM/yyyy"));
    }
  }

  getHeader(): string {
    switch (this.mode) {
      case SubscriptionDetailsMode.Registeration: return `${this.strings.chooseYourSubjects}`;
      case SubscriptionDetailsMode.MyAccount: return this.strings.myAccount;
      default: return '';
    }
  }

  getSubHeader(): string | null {
    let curiiculum = this.selectedCurriculum ?? this._subscriptionParameters?.curricula.find(x => x.isGeneric);
    if(curiiculum?.curriculumCode == 0 || curiiculum?.curriculumCode == 4) return null
    switch (this.mode) {
      case SubscriptionDetailsMode.Registeration: return curiiculum?.name ?? null;
      case SubscriptionDetailsMode.MyAccount: return null;
      default: return null;
    }
  }

  onSubmit() {
    if (!this.subscriptionParameters) {
      this.error("Subscription parameters cannot be null");
      return;
    }
    let curiiculum = this.selectedCurriculum ?? this._subscriptionParameters?.curricula.find(x => x.isGeneric);
    if (!curiiculum) {
      this.error("Curriculum error");
      return;
    }

    if (!this._subscriptionParameters?.selectedPaymentPeriod || !this.isSubjectSelected) {
      this.error("Not all paramenters selected");
      return;
    }

    //filter by filters
    // var courses = this.subscriptionParameters.products.filter(c => c.curriculumCode == curiiculum?.curriculumCode);
    var courses = this.subscriptionParameters.subjects.filter(_ => _.isSelected).map(_ => _.isNeedYearSelection ? _.selectedProduct : _.products[0]);
    //filter by selected subjects
    // courses = courses.filter(c => this.subscriptionParameters?.subjects.some(s => s.curriculumCode == c.curriculumCode && s.isSelected && s.subjectCode == c.subjectCode && (!s.isNeedYearSelection || s.year == c.year)));
    this.subscriptionParameters?.additionalSubjects
      .filter(c => this.subscriptionParameters?.additionalSubjects.some(s => s.isSelected && s.subjectCode == c.subjectCode))
      .forEach(c => courses.push(c.products[0]));
    console.log(courses);

    this.next.emit(
      new SubscriptionData(
        courses.map(_ => _.productId),
        this._subscriptionParameters?.selectedPaymentPeriod,
        this.total ?? 0,
        this._subscriptionParameters?.currency,
        curiiculum.curriculumCode,
        this._subscriptionParameters?.country
      )
    );
  }

  onUseVoucher() {
    if (!this.subscriptionParameters) {
      this.error("Subscription parameters cannot be null");
      return;
    }
    let curiiculum = this.selectedCurriculum ?? this._subscriptionParameters?.curricula.find(x => x.isGeneric);
    if (!curiiculum) {
      this.error("Curriculum error");
      return;
    }

    if (!this._subscriptionParameters?.selectedPaymentPeriod || !this.isSubjectSelected) {
      this.error("Not all paramenters selected");
      return;
    }

    if((this.total ?? 0) > (this._subscriptionParameters?.voucherAmount ?? 0)){
      this.error("Voucher amount is not enough");
      return;
    }

    //filter by filters
    // var courses = this.subscriptionParameters.products.filter(c => c.curriculumCode == curiiculum?.curriculumCode);
    var courses = this.subscriptionParameters.subjects.filter(_ => _.isSelected).map(_ => _.isNeedYearSelection ? _.selectedProduct : _.products[0]);
    //filter by selected subjects
    // courses = courses.filter(c => this.subscriptionParameters?.subjects.some(s => s.curriculumCode == c.curriculumCode && s.isSelected && s.subjectCode == c.subjectCode && (!s.isNeedYearSelection || s.year == c.year)));
    this.subscriptionParameters?.additionalSubjects
      .filter(c => this.subscriptionParameters?.additionalSubjects.some(s => s.isSelected && s.subjectCode == c.subjectCode))
      .forEach(c => courses.push(c.products[0]));
    console.log(courses);

    this.useVoucher.emit(
      new SubscriptionData(
        courses.map(_ => _.productId),
        this._subscriptionParameters?.selectedPaymentPeriod,
        this.total ?? 0,
        this._subscriptionParameters?.currency,
        curiiculum.curriculumCode,
        this._subscriptionParameters?.country
      )
    );
  }

  onStartFreeTrial() {
    if (!this.subscriptionParameters) {
      this.error("Subscription parameters cannot be null");
      return;
    }
    let curiiculum = this.selectedCurriculum ?? this._subscriptionParameters?.curricula.find(x => x.isGeneric);
    if (!curiiculum) {
      this.error("Curriculum error");
      return;
    }

    if (!this.isSubjectSelected) {
      this.error("Not all paramenters selected");
      return;
    }

    //filter by filters
    // var courses = this.subscriptionParameters.products.filter(c => c.curriculumCode == curiiculum?.curriculumCode);
    var courses = this.subscriptionParameters.subjects.filter(_ => _.isSelected).map(_ => _.isNeedYearSelection ? _.selectedProduct : _.products[0]);
    //filter by selected subjects
    // courses = courses.filter(c => this.subscriptionParameters?.subjects.some(s => s.curriculumCode == c.curriculumCode && s.isSelected && s.subjectCode == c.subjectCode && (!s.isNeedYearSelection || s.year == c.year)));
    this.subscriptionParameters?.additionalSubjects
      .filter(c => this.subscriptionParameters?.additionalSubjects.some(s => s.isSelected && s.subjectCode == c.subjectCode))
      .forEach(c => courses.push(c.products[0]));
    console.log(courses);

    this.freeTrial.emit(new FreeTrialData(courses.map(_ => _.productId)));
  }

  onCourseSelect(subject: GetSubscriptionParametersQuery_ResultSubject, sender: MatCheckbox) {
    let maxSubjNumber = Math.max(...(this._subscriptionParameters?.prices?.map(_ => _.subjectsNumber) ?? [0]));
    if (this.subjectsNumber <= maxSubjNumber) return;

    this.error(this.strings.errors.maxBundleSize(maxSubjNumber));
    subject.isSelected = false;
    sender.checked = false;
  }

  getYearName(curriculumCode: number, year: number){
    switch(curriculumCode){
      case 1:
        return year
      default:
        return "";
    }
  }

  onCancel() {
    this.cancel.emit();
  }

  onEditBillingDetails() {
    this.editBillingDetails.emit();
  }

  onCancelSubscription() {
    this.cancelSubscription.emit();
  }
}
