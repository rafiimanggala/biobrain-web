import { Injectable } from '@angular/core';

import { StringsService } from 'src/app/share/strings.service';
import { Api } from 'src/app/api/api.service';
import { Dialog } from 'src/app/core/dialogs/dialog.service';
import { AppEventProvider } from 'src/app/core/app/app-event-provider.service';
import { Result, SuccessOrFailedResult } from 'src/app/share/helpers/result';
import { ImageModel } from '../../services/summernote/image.model';
import { UserGuideIamgeDialog } from '../../user-guides/dialogs/user-guide-image/user-guide-image-dialog.component';
import { UserGuideImageDialogData } from '../../user-guides/dialogs/user-guide-image/user-guide-image-dialog-data';
import { firstValueFrom } from 'src/app/share/helpers/first-value-from';
import { UploadUserGuideImageCommand } from 'src/app/api/user-guides/upload-user-guide-image.command';
import { DialogAction } from 'src/app/core/dialogs/dialog-action';

@Injectable({
  providedIn: 'root',
})
export class AddImageOperation {

  constructor(
    private readonly _strings: StringsService,
    private readonly _api: Api,
    private readonly _appEvent: AppEventProvider,
    private readonly _dialog: Dialog,
  ) {
  }

  public async perform(): Promise<SuccessOrFailedResult<ImageModel>> {
    const dialogResult = await this._dialog.show(UserGuideIamgeDialog, new UserGuideImageDialogData(null));
    if (dialogResult.action == DialogAction.cancel) return Result.failed();
    if (!dialogResult.hasData()) return Result.failed();
    if (!dialogResult.data.fileToUpload) return Result.failed();

    try {
      var result = await firstValueFrom(this._api.sendFile(new UploadUserGuideImageCommand(dialogResult.data.fileToUpload)));
      if (!result) return Result.failed();;
      var image = new ImageModel();
      image.url = result.fileLink;
      return Result.success(image);
    } catch (e) {
      this._appEvent.errorEmit(this._strings.errors.errorSavingDataOnServer);
      return Result.failed();
    }
    return Result.success(new ImageModel());
  }
}
