import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Api } from 'src/app/api/api.service';
import { PaymentPeriod } from 'src/app/api/enums/payment-period.enum';
import { GetPromoCodeByCodeCommand, GetPromoCodeByCodeCommand_Result } from 'src/app/api/payments/get-promo-code-by-code.command';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';
import { PaymentStringsService } from 'src/app/payment/services/payment-strings.service';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { LoaderService } from 'src/app/share/services/loader.service';
import { appCountries } from 'src/app/share/values/countries';

import { StringsService } from '../../../../share/strings.service';
import { SubscriptionData } from '../subscription-details.component.ts/subscription.data';

import { PaymentConfirmationData } from './payment-confirmation-data';

@Component({
  selector: 'app-payment-confirmation',
  templateUrl: './payment-confirmation.component.html',
  styleUrls: ['./payment-confirmation.component.scss'],
})
export class PaymentConfirmationComponent extends BaseComponent {
  @Output() subscribe = new EventEmitter<PaymentConfirmationData>();
  @Output() cancel = new EventEmitter();
  @Input() subscriptionData: SubscriptionData | undefined;

  isTermsConfirmed: boolean = false;
  get termsHtmlString(): SafeHtml { return this.sanitizer.bypassSecurityTrustHtml(this.strings.html.termsOfServiceString) }

  promoCode: GetPromoCodeByCodeCommand_Result | null = null;
  promoCodeValue: string = ''
  promoCodeError: string = '';
  total: number = 0;

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly sanitizer: DomSanitizer,
    private readonly _loaderService: LoaderService,
    private _paymentStringsService: PaymentStringsService,
    private _datepipe: DatePipe,
    appEvents: AppEventProvider,
  ) {
    super(appEvents);
  }

  getFooterMessage(): string {
    return this.strings.myAccountSubscriptionMessage(
      `${this.subscriptionData?.curency ?? ''}${this.subscriptionData?.total ?? ''}`,
      this._paymentStringsService.date,
      this._paymentStringsService.getNounPeriodText(this.subscriptionData?.period ?? PaymentPeriod.Monthly),
      this._paymentStringsService.getPeriodText(this.subscriptionData?.period ?? PaymentPeriod.Monthly),
      this.subscriptionData?.country?.toLowerCase() === appCountries.Australia.toLowerCase()
    );
  }

  async onSubscribe() {
    this.subscribe.emit(new PaymentConfirmationData(this.promoCode?.promoCodeId ?? null));
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

  onRemovePromo() {
    this.promoCode = null;
    this.promoCodeValue = '';
  }

  getTotalInternal(): number {
    if (!this.subscriptionData) return 0;
    if (!this.promoCode) return this.subscriptionData.total;

    if (this.promoCode.amount) {
      let total = (this.subscriptionData.total - this.promoCode.amount);
      return Math.round(Math.max(0, total) * 100) / 100;
    }
    if (this.promoCode.percent) {
      let total = this.subscriptionData.total - (this.subscriptionData.total * this.promoCode.percent / 100);
      return Math.round(Math.max(0, total) * 100) / 100;
    }
    return this.subscriptionData.total;
  }

  onCancel() {
    this.cancel.emit();
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
