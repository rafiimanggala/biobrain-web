import { Injectable } from '@angular/core';
import { Api } from 'src/app/api/api.service';
import { GetTempaltesQuery } from 'src/app/api/templates/get-templates.query';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from 'src/app/share/dialogs/error-message-dialog/error-message-dialog.component';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { TemplateViewModel } from '../template.view-model';
import { DeleteConfirmationDialog } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog';
import { DeleteConfirmationDialogData } from 'src/app/share/dialogs/delete-confirmation/delete-confirmation.dialog-data';
import { StringsService } from 'src/app/share/strings.service';
import { DeleteTemplateCommand } from 'src/app/api/templates/delete-template.command';

@Injectable({
  providedIn: 'root',
})
export class DeleteTemplateOperation {

  constructor(
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _strings: StringsService,
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult<{}, string>> {
    return Result.success({});
  }

  public async perform(template: TemplateViewModel): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform();
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    var confirmation = await this._dialog.show(DeleteConfirmationDialog, new DeleteConfirmationDialogData(this._strings.template, template.template));
    if (!confirmation.data?.confirmed) {
      return Result.success();
    }

    await this._api.send(new DeleteTemplateCommand(template.templateId)).toPromise();
    return Result.success();
  }
}
