import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';

import { StringsService } from '../../../share/strings.service';
import { ResetSelfPasswordOperation } from '../../operations/reset-self-password.operation';
import { RoutingService } from '../../services/routing.service';

import { ResetPasswordData } from './reset-password-data';

@Component({
  selector: 'app-reset-password-page',
  templateUrl: './reset-password-page.component.html',
  styleUrls: ['./reset-password-page.component.scss'],
})
export class ResetPasswordPageComponent {
  data = new ResetPasswordData();

  constructor(
    public readonly strings: StringsService,
    private readonly _routingService: RoutingService,
    private readonly _askSelfPasswordResetOperation: ResetSelfPasswordOperation,
  ) {
  }

  async onSubmit(form: NgForm): Promise<void> {
    if (!form.valid) return;

    await this._askSelfPasswordResetOperation.perform(this.data.email);
    await this._routingService.navigateToLoginPage();
  }
}
