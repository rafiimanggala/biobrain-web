import { Injectable } from '@angular/core';
import { RoutingService } from 'src/app/auth/services/routing.service';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';
import { ConfirmationDialog } from 'src/app/share/dialogs/confirmation/confirmation.dialog';
import { ConfirmationDialogData } from 'src/app/share/dialogs/confirmation/confirmation.dialog-data';
import { StringsService } from 'src/app/share/strings.service';

import { Dialog } from '../../core/dialogs/dialog.service';

@Injectable({
  providedIn: 'root',
})
export class AssertNotAvailableInDemoOperation {
  constructor(
    private readonly _dialog: Dialog,
    private readonly _routingService: RoutingService,
    private readonly strings: StringsService
  ) {
  }

  public async perform(): Promise<void> {
    var result = await this._dialog.show(ConfirmationDialog, new ConfirmationDialogData(this.strings.freeTrial, this.strings.upgradeMessage, this.strings.subscribeNow, this.strings.cancel ));
    if(result.action != DialogAction.yes) return;
        
    await this._routingService.navigateToSubscriptionPage();
  }
 
}
