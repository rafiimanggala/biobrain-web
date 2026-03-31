import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { StringsService } from 'src/app/share/strings.service';

import { hasValue } from '../../../share/helpers/has-value';
import { LoaderService } from '../../../share/services/loader.service';
import { AdminLoginModel } from '../../models/admin-login.model';
import { AdminLoginOperation } from '../../operations/admin-login.operation';
import { RoutingService } from '../../services/routing.service';

@Component({
  selector: 'app-admin-login',
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.scss'],
})
export class AdminLoginComponent extends BaseComponent implements OnInit, OnDestroy {
  user: AdminLoginModel = new AdminLoginModel();
  private readonly _backUrl: string | null;

  constructor(
    appEvents: AppEventProvider,
    private readonly _routingService: RoutingService,
    public strings: StringsService,
    private readonly _loginOperation: AdminLoginOperation,
    _activatedRouteSnapshot: ActivatedRoute,
    private readonly _loaderService: LoaderService
  ) {
    super(appEvents);
    this._backUrl = _activatedRouteSnapshot.snapshot.queryParamMap.get('backUrl');
  }

  async ngOnInit(): Promise<void> {
  }

  public async onSubmit(form: NgForm): Promise<void> {
    if (!form.valid) {
      return;
    }

    this._loaderService.show();
    try {
      this.user.username = this.user.username.trim();
      const result = await this._loginOperation.perform(this.user);
      if (result.isFailed()) {
        this.error(result.reason);
        return;
      }
    } finally {
      this._loaderService.hide();
    }

    await this._navigateToNextPage();
  }

  private async _navigateToNextPage(): Promise<void> {
    // let user = await this._userService.user;
    // if (!user?.subscriptionStatus || (+user.subscriptionStatus == ScheduledPaymentStatus.Success || +user.subscriptionStatus == ScheduledPaymentStatus.StoppedByUser)) {
      if (hasValue(this._backUrl)) {
        await this._routingService.navigateToUrl(this._backUrl);
      } else {
        await this._routingService.navigateToHome();
      }
    // }
    // else{
    //   // ToDo: Navigate to payment
    //     await this._routingService.navigateToSubscriptionPage();
    // }
  }

  reload(){
    window.location.reload();
  }
}
