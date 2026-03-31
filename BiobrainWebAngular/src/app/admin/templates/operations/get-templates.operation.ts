import { Injectable } from '@angular/core';
import { Api } from 'src/app/api/api.service';
import { GetTempaltesQuery } from 'src/app/api/templates/get-templates.query';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from 'src/app/share/dialogs/error-message-dialog/error-message-dialog.component';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { TemplateViewModel } from '../template.view-model';

@Injectable({
  providedIn: 'root',
})
export class TakeTemplatesOperation {

  constructor(
    private readonly _api: Api,
    private readonly _dialog: Dialog,
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult<{}, string>> {
    return Result.success({});
  }

  public async perform(): Promise<FailedOrSuccessResult<undefined, TemplateViewModel[]>> {
    const canPerformResult = await this.canPerform();
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    const getResult = await this._api.send(new GetTempaltesQuery()).toPromise();
    return Result.success(getResult.map(_ => new TemplateViewModel(
      _.templateId, 
      _.template,
      _.templateType, 
      _.courses.map(c => {
        return {id: c.courseId, name: c.name};
      }))));
  }
}
