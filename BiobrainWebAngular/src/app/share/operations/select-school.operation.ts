import { Injectable } from '@angular/core';
import { GetSchoolListQuery, SchoolListModel_Result } from 'src/app/api/schools/get-school-list.query';
import { BadRequestCommonException } from 'src/app/core/exceptions/bad-request-common.exception';
import { InternalServerException } from 'src/app/core/exceptions/internal-server.exception';
import { RequestValidationException, validationExceptionToString } from 'src/app/core/exceptions/request-validation.exception';

import { Api } from '../../api/api.service';
import { CurrentUserService } from '../../auth/services/current-user.service';
import { Dialog } from '../../core/dialogs/dialog.service';
import { SelectItemDialogData } from '../dialogs/select-school-dialog/select-item-dialog-data';
import { SelectItemDialog } from '../dialogs/select-school-dialog/select-item-dialog.component';
import { firstValueFrom } from '../helpers/first-value-from';
import { hasValue } from '../helpers/has-value';
import { Result, SuccessOrFailedResult } from '../helpers/result';
import { LoaderService } from '../services/loader.service';
import { SnackBarService } from '../services/snack-bar.service';
import { StringsService } from '../strings.service';

@Injectable({
  providedIn: 'root',
})
export class SelectSchoolOperation {
  constructor(
    private readonly _strings: StringsService,
    private readonly _dialog: Dialog,
    private readonly _api: Api,
    private readonly _loaderService: LoaderService,
    private readonly _currentUserService: CurrentUserService,
    private readonly _snackBarService: SnackBarService,
  ) {
  }

  public async canPerform(): Promise<SuccessOrFailedResult> {
    const user = await this._currentUserService.user;
    if (!hasValue(user)) return Result.failed();
    if (!user.isSchoolAdmin()) return Result.failed();

    return Result.success();
  }

  async perform(ids: string[]): Promise<SuccessOrFailedResult<string>> {
    const canPerform = await this.canPerform();
    if (canPerform.isFailed()) {
      this._snackBarService.showMessage(this._strings.accessDenied);
      return Result.failed();
    }

    var schools: SchoolListModel_Result[] = [];

    try {
      this._loaderService.show();
      schools = await firstValueFrom(this._api.send(new GetSchoolListQuery(ids)));
    } catch (e) {
      if (e instanceof BadRequestCommonException) this._snackBarService.showMessage(e.message);
      else if (e instanceof InternalServerException) this._snackBarService.showMessage(e.message);
      else if (e instanceof RequestValidationException) this._snackBarService.showMessage(validationExceptionToString(e));
      else this._snackBarService.showMessage(this._strings.error);
      return Result.failed();
    } finally {
      this._loaderService.hide();
    }
    if(!schools) 
      return Result.failed();

    var result = await this._dialog.show(SelectItemDialog, new SelectItemDialogData(this._strings.school, schools.map(x => {return {id: x.schoolId, name: x.name};})));

    return result.hasData() ? Result.success(result.data?.id ) : Result.failed();
  }
}
