import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Api } from 'src/app/api/api.service';
import { GetLatestWhatsNewQuery, GetLatestWhatsNewQuery_Result } from 'src/app/api/whats-new/get-latest-whats-new.query';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { AuthTokensService } from 'src/app/core/storage/auth-tokens.service';
import { StarRatingDialogComponent } from 'src/app/share/dialogs/star-rating-dialog/star-rating-dialog.component';
import { StarRatingDialogData } from 'src/app/share/dialogs/star-rating-dialog/star-rating-dialog-data';
import { WhatsNewDialogComponent } from 'src/app/share/dialogs/whats-new-dialog/whats-new-dialog.component';
import { WhatsNewDialogData } from 'src/app/share/dialogs/whats-new-dialog/whats-new-dialog-data';
import { StringsService } from 'src/app/share/strings.service';

import { hasValue } from '../../../share/helpers/has-value';
import { LoaderService } from '../../../share/services/loader.service';
import { LoginModel } from '../../models/login.model';
import { LoginOperation } from '../../operations/login.operation';
import { CurrentUserService } from '../../services/current-user.service';
import { RoutingService } from '../../services/routing.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent extends BaseComponent implements OnInit, OnDestroy {
  user: LoginModel = new LoginModel();
  private readonly _backUrl: string | null;

  // SSO state
  showSsoPanel = false;
  ssoSchoolName = '';
  ssoSearching = false;
  ssoError = '';
  ssoSchool: { schoolId: string; name: string } | null = null;

  private static readonly LOGIN_COUNT_KEY = 'biobrain_login_count';
  private static readonly HAS_RATED_KEY = 'biobrain_has_rated';
  private static readonly RATING_TRIGGERS = [4, 10];
  private static readonly LAST_SEEN_VERSION_KEY = 'biobrain_last_seen_version';

  constructor(
    appEvents: AppEventProvider,
    private readonly _routingService: RoutingService,
    public strings: StringsService,
    private readonly _loginOperation: LoginOperation,
    private readonly _authTokensService: AuthTokensService,
    _activatedRouteSnapshot: ActivatedRoute,
    private readonly _loaderService: LoaderService,
    private readonly _userService: CurrentUserService,
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _http: HttpClient
  ) {
    super(appEvents);
    this._backUrl = _activatedRouteSnapshot.snapshot.queryParamMap.get('backUrl');
  }

  async ngOnInit(): Promise<void> {
    // Handle SSO callback with code
    const params = new URLSearchParams(window.location.search);
    const ssoCode = params.get('ssoCode');
    const ssoErrorParam = params.get('ssoError');

    if (ssoErrorParam) {
      this.ssoError = decodeURIComponent(ssoErrorParam);
    }

    if (ssoCode) {
      await this._handleSsoCode(ssoCode);
      return;
    }

    if (this._authTokensService.getTokens()) {
      await this._navigateToNextPage();
    }
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

    await this._checkAndShowWhatsNew();
    await this._checkAndShowRatingDialog();
    await this._navigateToNextPage();
  }

  async onRestore(): Promise<void> {
    await this._routingService.navigateToResetPassword();
  }

  async onSignUp(): Promise<void> {
    await this._routingService.navigateToSignUp();
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

  reload(): void {
    window.location.reload();
  }

  private async _checkAndShowRatingDialog(): Promise<void> {
    const hasRated = localStorage.getItem(LoginComponent.HAS_RATED_KEY) === 'true';
    if (hasRated) {
      return;
    }

    const currentCount = parseInt(localStorage.getItem(LoginComponent.LOGIN_COUNT_KEY) || '0', 10);
    const newCount = currentCount + 1;
    localStorage.setItem(LoginComponent.LOGIN_COUNT_KEY, String(newCount));

    if (!LoginComponent.RATING_TRIGGERS.includes(newCount)) {
      return;
    }

    const result = await this._dialog.show(
      StarRatingDialogComponent,
      new StarRatingDialogData(),
      { disableClose: false, width: '420px' }
    );

    if (result && result.action === DialogAction.save) {
      localStorage.setItem(LoginComponent.HAS_RATED_KEY, 'true');
    }
  }

  private async _checkAndShowWhatsNew(): Promise<void> {
    try {
      const data = await this._api.send(new GetLatestWhatsNewQuery()).toPromise();
      if (!data || !data.whatsNewId || !data.version) {
        return;
      }

      const lastSeen = localStorage.getItem(LoginComponent.LAST_SEEN_VERSION_KEY);
      if (lastSeen === data.version) {
        return;
      }

      await this._dialog.show(
        WhatsNewDialogComponent,
        new WhatsNewDialogData(data.title, data.content, data.version),
        { disableClose: false, width: '560px' }
      );

      localStorage.setItem(LoginComponent.LAST_SEEN_VERSION_KEY, data.version);
    } catch {
      // Silently ignore - don't block login flow
    }
  }

  // --- SSO Methods ---

  toggleSsoPanel(): void {
    this.showSsoPanel = !this.showSsoPanel;
    this.ssoError = '';
    this.ssoSchool = null;
    this.ssoSchoolName = '';
  }

  async searchSsoSchool(): Promise<void> {
    if (!this.ssoSchoolName || this.ssoSchoolName.trim().length < 2) {
      this.ssoError = '';
      this.ssoSchool = null;
      return;
    }

    this.ssoSearching = true;
    this.ssoError = '';
    this.ssoSchool = null;

    try {
      const result = await this._http
        .get<{ schoolId: string; name: string }>(`/api/auth/saml/lookup?schoolName=${encodeURIComponent(this.ssoSchoolName.trim())}`)
        .toPromise();
      this.ssoSchool = result;
    } catch {
      this.ssoSchool = null;
      this.ssoError = this.strings.ssoNoSchoolFound;
    } finally {
      this.ssoSearching = false;
    }
  }

  startSsoLogin(): void {
    if (!this.ssoSchool) {
      return;
    }
    // Redirect to SAML login endpoint — this will redirect to the IdP
    window.location.href = `/api/auth/saml/login?schoolId=${this.ssoSchool.schoolId}`;
  }

  private async _handleSsoCode(code: string): Promise<void> {
    this._loaderService.show();
    try {
      const tokens = await this._http
        .post<{ access_token: string; refresh_token: string }>('/api/auth/saml/exchange', { code })
        .toPromise();

      if (tokens && tokens.access_token) {
        this._authTokensService.setTokens(tokens);
        await this._navigateToNextPage();
      } else {
        this.ssoError = this.strings.ssoError;
      }
    } catch {
      this.ssoError = this.strings.ssoError;
    } finally {
      this._loaderService.hide();
    }
  }
}
