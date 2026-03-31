import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

import { AppEventProvider } from '../../../core/app/app-event-provider.service';
import { hasValue } from '../../../share/helpers/has-value';
import { SnackBarService } from '../../../share/services/snack-bar.service';
import { StringsService } from '../../../share/strings.service';
import { SetPasswordOperation } from '../../operations/set-password.operation';
import { RoutingService } from '../../services/routing.service';

import { SetPasswordData } from './set-password-data';

@Component({
  selector: 'app-set-password-page',
  templateUrl: './set-password-page.component.html',
  styleUrls: ['./set-password-page.component.scss'],
})
export class SetPasswordPageComponent {
  data = new SetPasswordData();

  public registration = false;
  public login: string | undefined | null;
  public token: string | undefined | null;

  constructor(
    public readonly strings: StringsService,
    private readonly _setPasswordOperation: SetPasswordOperation,
    private readonly _appEvents: AppEventProvider,
    readonly _route: ActivatedRoute,
    private readonly _routingService: RoutingService,
  ) {
    this.registration = _route.snapshot.queryParamMap.has('registration');
    this.login = _route.snapshot.queryParamMap.get('login');
    this.token = _route.snapshot.queryParamMap.get('token');
  }

  async onSubmit(form: NgForm): Promise<void> {
    if (!form.valid) return;

    if (!hasValue(this.login) || !hasValue(this.token)) {
      this._appEvents.errorEmit(this.strings.missParametersError);
      await this._routingService.navigateToLoginPage();
      return;
    }

    await this._setPasswordOperation.perform(this.login, this.token, this.data.password);
    await this._routingService.navigateToLoginPage();
  }
}
