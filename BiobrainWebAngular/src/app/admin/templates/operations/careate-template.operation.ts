import { Injectable } from '@angular/core';
import { Api } from 'src/app/api/api.service';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { ErrorMessageDialogComponent } from 'src/app/share/dialogs/error-message-dialog/error-message-dialog.component';
import { FailedOrSuccessResult, Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { StringsService } from 'src/app/share/strings.service';
import { TemplateDialog } from '../dialog/template-dialog.component';
import { TemplateDialogData } from '../dialog/template-dialog-data';
import { GetCoursesListQuery } from 'src/app/api/content/get-courses-list.query';
import { SaveTemplateCommand } from 'src/app/api/templates/save-template.command';

@Injectable({
  providedIn: 'root',
})
export class CreateTemplateOperation {

  constructor(
    private readonly _api: Api,
    private readonly _dialog: Dialog,
    private readonly _strings: StringsService,
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult<{}, string>> {
    return Result.success({});
  }

  public async perform(): Promise<FailedOrSuccessResult> {
    const canPerformResult = await this.canPerform();
    if (canPerformResult.isFailed()) {
      await this._dialog.show(ErrorMessageDialogComponent, { text: canPerformResult.reason });
      return Result.failed();
    }

    var courses = await this._api.send(new GetCoursesListQuery()).toPromise();

    var types = [
      {key: 1, value: "Class Result Quiz Header"},
      {key: 2, value: "Bookmark Path Header"},
      {key: 3, value: "Quiz Results Quiz Header"},
    ];

    var data = await this._dialog.show(TemplateDialog, new TemplateDialogData(courses.map(_ => {return {courseId: _.courseId, name: _.name, isSelected: false}}), types, null, "", 1, []));
    if(!data.data) return Result.failed();
    
    await this._api.send(new SaveTemplateCommand(null, data.data.template, data.data.templateType, data.data.courseIds)).toPromise();
    return Result.success();
  }
}
