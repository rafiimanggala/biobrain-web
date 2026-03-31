import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Api } from 'src/app/api/api.service';
import { GetCardTokenQuery } from 'src/app/api/payments/get-card-token.query';
import { GetPromoCodeByCodeCommand, GetPromoCodeByCodeCommand_Result } from 'src/app/api/payments/get-promo-code-by-code.command';
import { GetPaymentMethodsQuery_Result } from 'src/app/api/payments/get-pyment-methods.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { BadRequestCommonException } from 'src/app/core/exceptions/bad-request-common.exception';
import { InternalServerException } from 'src/app/core/exceptions/internal-server.exception';
import { RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';
import { Countries, worldCountries } from 'src/app/share/values/countries';

import { StringsService } from '../../../../share/strings.service';
import { SubscriptionData } from '../subscription-details.component.ts/subscription.data';
import { PaymentDetailsCard } from './payment-details-card.model';
import { PaymentDetailsData } from './payment-details-data';

export enum PaymentDetailsMode {
  Registeration = 1,
  MyAccount = 2
}

@Component({
  selector: 'app-payment-details',
  templateUrl: './payment-details.component.html',
  styleUrls: ['./payment-details.component.scss'],
})
export class PaymentDetailsComponent extends BaseComponent {
  @Output() subscribe = new EventEmitter<PaymentDetailsData>();
  @Output() cancel = new EventEmitter();
  @Input() subscriptionData: SubscriptionData | undefined;
  @Input() paymentMethods: GetPaymentMethodsQuery_Result[] = [];
  @Input() mode: PaymentDetailsMode = PaymentDetailsMode.Registeration;
  @Input() signup: boolean = false;
  card: PaymentDetailsCard = new PaymentDetailsCard();
  isTermsConfirmed: boolean = false;
  get termsHtmlString(): SafeHtml { return this.sanitizer.bypassSecurityTrustHtml(this.strings.html.termsOfServiceString) }
  filtredCountries: Countries[] = [];
  promoCode: GetPromoCodeByCodeCommand_Result | null = null;
  promoCodeValue: string = '';
  promoCodeError: string = '';
  total: number = 0;

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly sanitizer: DomSanitizer,
    private readonly _loaderService: LoaderService,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);
    this.filtredCountries = worldCountries;
  }

  async onSubscribe() {
    if (this.paymentMethods.length < 1) {
      this.error("No payment methods");
      return;
    }

    try {
      this._loaderService.show();
      let card = await this._api.send(new GetCardTokenQuery(
        this.card.cardNumber?.toString() ?? "",
        this.card.expiryMonth ?? 0,
        this.card.expiryYear ?? 0,
        this.card.cvc ?? '',
        this.card.cardholderName ?? '',
        this.card.addressLine1 ?? '',
        this.card.city ?? '',
        this.card.country ?? '')).toPromise();
      this.subscribe.emit(new PaymentDetailsData(card.cardToken, this.promoCode?.promoCodeId ?? null));
    }
    catch (e) {
      if (e instanceof BadRequestCommonException) this.error(e.message);
      if (e instanceof InternalServerException) this.error(e.message);
      if (e instanceof RequestValidationException) this.error(validationExceptionToString(e));
    }
    finally {
      this._loaderService.hideIfVisible();
    }
  }

  async onApplyPromoCode() {
    if (!this.promoCodeValue || this.promoCodeValue.length < 3) return;
    if (!this.subscriptionData) return;
    try {
      this._loaderService.show();
      var result = await firstValueFrom(this._api.send(new GetPromoCodeByCodeCommand(this.promoCodeValue, this.subscriptionData.period, this.subscriptionData.courseIds.length)));
      if (!result) return;

      this.promoCode = result;
    }
    catch (e) {
      if (e instanceof RequestValidationException) {
        this.error(validationExceptionToString(e));
      }
      console.log(e);
    }
    finally {
      this._loaderService.hideIfVisible();
    }
  }

  onRemovePromo(){
    this.promoCode = null;
    this.promoCodeValue = '';
  }

  getTotalInternal(): number {
    if (!this.subscriptionData) return 0;
    if (!this.promoCode) return this.subscriptionData.total;

    if (this.promoCode.amount) {
      let total = (this.subscriptionData.total - this.promoCode.amount);
      return Math.round(Math.max(0, total)*100)/100;
    }
    if (this.promoCode.percent) {
      let total = this.subscriptionData.total - (this.subscriptionData.total*this.promoCode.percent/100);
      return Math.round(Math.max(0, total)*100)/100;
    }
    return this.subscriptionData.total;
  }

  onCancel() {
    this.cancel.emit();
  }

  onCountryChanged() {
    if (!this.card.country) {
      this.filtredCountries = worldCountries;
      return;
    }
    var filter = this.card.country.toLowerCase();
    this.filtredCountries = worldCountries.filter(x => x.name.toLowerCase().includes(filter));
  }

  getPromoCodeValueInternal() {
    if (!this.promoCode) return '';
    return this.promoCode.percent
      ? `-${this.promoCode.percent}%`
      : this.promoCode.amount
        ? `-${this.subscriptionData?.curency}${this.promoCode.amount}`
        : '';
  }
}
