import { Component, OnDestroy, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { BaseComponent } from 'src/app/core/app/base.component';
import { AuthTokensService } from 'src/app/core/storage/auth-tokens.service';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { StringsService } from 'src/app/share/strings.service';
import { StarRatingDialogComponent, STAR_RATING_RESULT_PREFIX } from 'src/app/share/dialogs/star-rating-dialog/star-rating-dialog.component';
import { StarRatingDialogData } from 'src/app/share/dialogs/star-rating-dialog/star-rating-dialog-data';

import { Api } from '../../../api/api.service';
import { GetLatestWhatsNewQuery, GetLatestWhatsNewQuery_Result } from '../../../api/whats-new/get-latest-whats-new.query';
import { firstValueFrom } from '../../../share/helpers/first-value-from';
import { hasValue } from '../../../share/helpers/has-value';
import { LoaderService } from '../../../share/services/loader.service';
import { WhatsNewDialogComponent } from '../../../share/dialogs/whats-new-dialog/whats-new-dialog.component';
import { WhatsNewDialogData } from '../../../share/dialogs/whats-new-dialog/whats-new-dialog-data';
import { LoginModel } from '../../models/login.model';
import { LoginOperation } from '../../operations/login.operation';
import { CurrentUserService } from '../../services/current-user.service';
import { RoutingService } from '../../services/routing.service';

const STAR_RATING_TRIGGER_LOGINS = [4, 10];
const STAR_RATING_STORAGE_PREFIX = 'biobrain.loginCount.';
const STAR_RATING_SHOWN_PREFIX = 'biobrain.starRatingShown.';
const WHATS_NEW_SHOWN_PREFIX = 'biobrain.whatsNewShown.';

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

  constructor(
    appEvents: AppEventProvider,
    private readonly _routingService: RoutingService,
    public strings: StringsService,
    private readonly _loginOperation: LoginOperation,
    private readonly _authTokensService: AuthTokensService,
    _activatedRouteSnapshot: ActivatedRoute,
    private readonly _loaderService: LoaderService,
    private readonly _userService: CurrentUserService,
    private readonly _http: HttpClient,
    private readonly _dialog: Dialog,
    private readonly _api: Api,
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

    await this._trackLoginAndMaybeShowRating();
    await this._maybeShowWhatsNew();
    await this._navigateToNextPage();
  }

  private async _maybeShowWhatsNew(): Promise<void> {
    try {
      const currentUser = await this._userService.user;
      if (!hasValue(currentUser) || !hasValue(currentUser.userId)) return;

      const result: GetLatestWhatsNewQuery_Result = await firstValueFrom(
        this._api.send(new GetLatestWhatsNewQuery())
      );

      if (!hasValue(result) || !hasValue(result.whatsNewId) || !hasValue(result.version)) return;

      const shownKey = WHATS_NEW_SHOWN_PREFIX + currentUser.userId;
      const shownRaw = localStorage.getItem(shownKey);
      const shownVersions: string[] = shownRaw ? JSON.parse(shownRaw) : [];

      if (shownVersions.indexOf(result.version) !== -1) return;

      shownVersions.push(result.version);
      localStorage.setItem(shownKey, JSON.stringify(shownVersions));

      // Fire and forget — don't block navigation
      void this._dialog.show(
        WhatsNewDialogComponent,
        new WhatsNewDialogData(result.title, result.content, result.version),
      );
    } catch {
      // Never block login on what's new errors
    }
  }

  private async _trackLoginAndMaybeShowRating(): Promise<void> {
    try {
      const currentUser = await this._userService.user;
      if (!hasValue(currentUser) || !hasValue(currentUser.userId)) return;

      const userId = currentUser.userId;
      const countKey = STAR_RATING_STORAGE_PREFIX + userId;
      const shownKey = STAR_RATING_SHOWN_PREFIX + userId;
      const resultKey = STAR_RATING_RESULT_PREFIX + userId;

      // Skip if user previously rated 4-5 stars
      const previousRating = parseInt(localStorage.getItem(resultKey) || '0', 10) || 0;
      if (previousRating >= 4) return;

      const previousCount = parseInt(localStorage.getItem(countKey) || '0', 10) || 0;
      const nextCount = previousCount + 1;
      localStorage.setItem(countKey, String(nextCount));

      if (STAR_RATING_TRIGGER_LOGINS.indexOf(nextCount) === -1) return;

      const shownRaw = localStorage.getItem(shownKey);
      const shownLogins: number[] = shownRaw ? JSON.parse(shownRaw) : [];
      if (shownLogins.indexOf(nextCount) !== -1) return;

      shownLogins.push(nextCount);
      localStorage.setItem(shownKey, JSON.stringify(shownLogins));

      // Build user display name: First Name + Last Initial
      const firstName = currentUser.firstName || '';
      const lastName = currentUser.lastName || '';
      const displayName = firstName + (lastName ? ' ' + lastName.charAt(0) + '.' : '');

      const dialogResult = await this._dialog.show(
        StarRatingDialogComponent,
        new StarRatingDialogData('Rate BioBrain', displayName.trim()),
      );

      // Store rating to skip future popups for 4-5 star ratings
      if (dialogResult && dialogResult.hasData() && dialogResult.data.rating >= 4) {
        localStorage.setItem(resultKey, String(dialogResult.data.rating));
      }
    } catch {
      // Never block login on rating dialog errors
    }
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
