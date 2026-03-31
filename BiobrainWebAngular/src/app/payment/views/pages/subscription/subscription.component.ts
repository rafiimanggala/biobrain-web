import { Component, OnInit } from "@angular/core";
import { Api } from "src/app/api/api.service";
import { SaveScheduledPaymentCommand } from "src/app/api/payments/save-subscription.comnand";
import { GetPaymentMethodsQuery, GetPaymentMethodsQuery_Result } from "src/app/api/payments/get-pyment-methods.query";
import { GetSubscriptionParametersQuery, GetSubscriptionParametersQuery_Result } from "src/app/api/payments/get-subscription-parameters.query";
import { LogoutOperation } from "src/app/auth/operations/logout.operation";
import { RefreshAuthorizationOperation } from "src/app/auth/operations/refresh-authorization.operation";
import { CurrentUser } from "src/app/auth/services/current-user";
import { CurrentUserService } from "src/app/auth/services/current-user.service";
import { RoutingService } from "src/app/auth/services/routing.service";
import { AppEventProvider } from "src/app/core/app/app-event-provider.service";
import { BaseComponent } from "src/app/core/app/base.component";
import { BadRequestCommonException } from "src/app/core/exceptions/bad-request-common.exception";
import { InternalServerException } from "src/app/core/exceptions/internal-server.exception";
import { RequestValidationException, validationExceptionToString } from "src/app/core/exceptions/request-validation.exception";
import { LoaderService } from "src/app/share/services/loader.service";
import { StringsService } from "src/app/share/strings.service";
import { SubscriptionData } from "../../components/subscription-details.component.ts/subscription.data";
import { Subscription } from "rxjs";
import { ActivatedRoute } from "@angular/router";
import { PaymentDetailsData } from "../../components/payment-details/payment-details-data";
import { EditUserProfileOperation } from "src/app/share/operations/edit-user-profile.operation";
import { AppSettings } from "src/app/share/values/app-settings";
import { FreeTrialData } from "../../components/subscription-details.component.ts/free-trial.data";
import { AddTrialScheduledPaymentCommand } from "src/app/api/payments/add-trial-subscription.command";
import { PaymentPeriod } from "src/app/api/enums/payment-period.enum";
import { AddVoucherScheduledPaymentCommand } from "src/app/api/payments/add-voucher-subscription.command";


@Component({
  selector: 'app-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss'],
})
export class SubscriptionComponent extends BaseComponent implements OnInit {
  subscriptionParameters: GetSubscriptionParametersQuery_Result | undefined;
  paymentMethods: GetPaymentMethodsQuery_Result[] = [];
  subscriptionDetails: SubscriptionData | undefined;
  currentStep: SubscriptionStep = SubscriptionStep.Subscription;
  user: CurrentUser | undefined;
  userChangesSubscription: Subscription | undefined;
  signup: boolean;
  isFreeTrial: boolean = false;//AddTrialScheduledPaymentCommand

  constructor(
    public readonly strings: StringsService,
    private readonly _api: Api,
    private readonly _userService: CurrentUserService,
    private readonly _routingService: RoutingService,
    private readonly _appEvents: AppEventProvider,
    private readonly _loaderService: LoaderService,
    private readonly _refreshTokenOperation: RefreshAuthorizationOperation,
    private readonly _editUserProfileOperation: EditUserProfileOperation,
    activatedRoute: ActivatedRoute,
    private readonly _logoutOperation: LogoutOperation
  ) {
    super(_appEvents);
    this.signup = Boolean(JSON.parse(activatedRoute.snapshot.queryParamMap.get('signup') ?? 'false'));
    this.isFreeTrial = activatedRoute.snapshot.url[0].path == AppSettings.freeTrialPath;
  }

  async ngOnInit() {
    this.user = await this._userService.user;
    if (!this.user) {
      this.error(this.strings.errors.noCurrentUser);
      return;
    }
    if (this.user.isStudent() && this.user.isSubscriptionValid() && (!this.user.isDemoSubscription() || this.isFreeTrial) || !this.user.isStudent()) {
      this._routingService.navigateToHome();
      return;
    }

    this.subscriptions.push(this._api.send(new GetSubscriptionParametersQuery(this.user.userId)).subscribe(x => {
      this.subscriptionParameters = x;
      if((this.subscriptionParameters?.voucherAmount ?? 0) > 0) this.subscriptionParameters.selectedPaymentPeriod = PaymentPeriod.Yearly;
    }));
    this.subscriptions.push(this._api.send(new GetPaymentMethodsQuery()).subscribe(x => this.paymentMethods = x));
  }

  onNext(data: SubscriptionData) {
    if (!data) {
      this.error("Subscription data can't be null");
      return;
    }

    this.subscriptionDetails = data;
    this.currentStep = SubscriptionStep.Payment;
  }

  async onFreeTrial(data: FreeTrialData) {
    if (!data) {
      this.error("Subscription data can't be null");
      return;
    }

    if (!this.user) {
      this.error(this.strings.errors.noCurrentUser);
      return;
    }

    if (!this.isFreeTrial) {
      return;
    }
    
    try {
      this._loaderService.show();
      await this._api.send(new AddTrialScheduledPaymentCommand(this.user.userId, data.courseIds)).toPromise();
      await this._refreshTokenOperation.perform();
      // this._snackBarService.showMessage(this.strings.youHaveSuccessfullyRegistered);   
      this._routingService.navigateToHome(this.signup);
    }
    catch (e) {
      if (e instanceof BadRequestCommonException) this.error(e.message);
      if (e instanceof InternalServerException) this.error(e.message);
      if (e instanceof RequestValidationException) this.error(validationExceptionToString(e));
      return;
    }
    finally {
      this._loaderService.hideIfVisible();
    }
    // ToDO Send req  uest
  }  

  async onUseVoucher(data: SubscriptionData) {
    if (!data) {
      this.error("Subscription data can't be null");
      return;
    }

    if (!this.user) {
      this.error(this.strings.errors.noCurrentUser);
      return;
    }

    if ((this.subscriptionParameters?.voucherAmount ?? 0) <= 0 || !this.subscriptionParameters?.voucherId) {
      return;
    }
    
    try {
      this._loaderService.show();
      await this._api.send(new AddVoucherScheduledPaymentCommand(this.user.userId, data.courseIds, this.subscriptionParameters.voucherId)).toPromise();
      await this._refreshTokenOperation.perform();
      // this._snackBarService.showMessage(this.strings.youHaveSuccessfullyRegistered);   
      this._routingService.navigateToHome(this.signup);
    }
    catch (e) {
      if (e instanceof BadRequestCommonException) this.error(e.message);
      if (e instanceof InternalServerException) this.error(e.message);
      if (e instanceof RequestValidationException) this.error(validationExceptionToString(e));
      return;
    }
    finally {
      this._loaderService.hideIfVisible();
    }
  }

  async onSubscribe(paymentData: PaymentDetailsData) {
    if (!paymentData?.cardToken) {
      this.error("Card data can't be null");
      return;
    }

    if (!this.user) {
      this.error(this.strings.errors.noCurrentUser);
      return;
    }

    if (!this.subscriptionDetails) {
      this.error("Subscription data can't be null");
      return;
    }

    try {
      this._loaderService.show();
      await this._api.send(new SaveScheduledPaymentCommand(this.user.userId, paymentData.cardToken, this.subscriptionDetails.courseIds, this.subscriptionDetails.period, paymentData.promocode)).toPromise();
      await this._refreshTokenOperation.perform();
      // this._snackBarService.showMessage(this.strings.youHaveSuccessfullyRegistered);   
      this._routingService.navigateToHome(this.signup);
    }
    catch (e) {
      if (e instanceof BadRequestCommonException) this.error(e.message);
      if (e instanceof InternalServerException) this.error(e.message);
      if (e instanceof RequestValidationException) this.error(validationExceptionToString(e));
      return;
    }
    finally {
      this._loaderService.hideIfVisible();
    }
  }

  async navigateHome() {
    // if(this.userChangesSubscription) this.userChangesSubscription.unsubscribe();
  }

  async onLogout() {
    await this._logoutOperation.perform();
  }

  async onEditProfile() {
    if (!this.user) return;
    await this._editUserProfileOperation.perform();
    this.subscriptions.push(this._api.send(new GetSubscriptionParametersQuery(this.user.userId)).subscribe(x => this.subscriptionParameters = x));
  }
}

export enum SubscriptionStep {
  Subscription = 0,
  Payment = 1
}
