import { Injectable } from "@angular/core";
import { Api } from "src/app/api/api.service";
import { GetSubscriptionParametersQuery } from "src/app/api/payments/get-subscription-parameters.query";
import { CurrentUserService } from "src/app/auth/services/current-user.service";
import { AppEventProvider } from "src/app/core/app/app-event-provider.service";
import { DialogAction } from "src/app/core/dialogs/dialog-action";
import { Dialog } from "src/app/core/dialogs/dialog.service";
import { BadRequestCommonException } from "src/app/core/exceptions/bad-request-common.exception";
import { InternalServerException } from "src/app/core/exceptions/internal-server.exception";
import { RequestValidationException, validationExceptionToString } from "src/app/core/exceptions/request-validation.exception";
import { firstValueFrom } from "src/app/share/helpers/first-value-from";
import { Result, SuccessOrFailedResult } from "src/app/share/helpers/result";
import { LoaderService } from "src/app/share/services/loader.service";
import { SnackBarService } from "src/app/share/services/snack-bar.service";
import { StringsService } from "src/app/share/strings.service";
import { SubscriptionDialog } from "../views/dialogs/subscription/subscription.dialog";
import { AddScheduledPaymentCommand } from "src/app/api/payments/add-subscription.comnand";
import { PaymentPeriod } from "src/app/api/enums/payment-period.enum";

@Injectable({
    providedIn: 'root',
})
export class AddSubscriptionOperation {
    constructor(
        private readonly _strings: StringsService,
        private readonly _dialog: Dialog,
        private readonly _api: Api,
        private readonly _loaderService: LoaderService,
        private readonly _currentUserService: CurrentUserService,
        private readonly _snackBarService: SnackBarService,
        private readonly _appEvents: AppEventProvider,
    ) {
    }

    public async canPerform(): Promise<SuccessOrFailedResult> {
        var user = await this._currentUserService.user;
        return user?.isStudent() ? Result.success() : Result.failed();
    }

    async perform(): Promise<SuccessOrFailedResult> {
        if (!(await this.canPerform())) return Result.failed();

        const student = await this._currentUserService.user;
        if (!student) return Result.failed();
        
        const dialogData = await firstValueFrom(this._api.send(new GetSubscriptionParametersQuery(student.userId)));
        if(!dialogData) return Result.failed();
        // Edit subscription details
        const dialogResult = await this._dialog.show(SubscriptionDialog, dialogData, {panelClass: "bordered-dialog-panel"});
        if(!dialogResult || !dialogResult.data) {
            return Result.failed();
        }

        switch(dialogResult.action){
            case DialogAction.cancel: return Result.success();
            case DialogAction.close: return Result.success();
            // Save subscription details
            case DialogAction.save: return await this.save(student.userId, dialogResult.data.subscriptionData.courseIds, dialogResult.data.subscriptionData.period, dialogResult.data.promocode.promocode);
            default: return Result.failed();
        }
    }

    private async save(userId: string, courses: string[], period: PaymentPeriod, promoCodeId: string|null){
        try {
            this._loaderService.show();
            await this._api.send(new AddScheduledPaymentCommand(userId,courses, period, promoCodeId)).toPromise();
            this._snackBarService.showMessage(this._strings.success);
            // await this._refreshTokenOperation.perform();
          }
          catch (e) {
            if (e instanceof BadRequestCommonException) this._appEvents.errorEmit(e.message);
            if (e instanceof InternalServerException) this._appEvents.errorEmit(e.message);
            if (e instanceof RequestValidationException) this._appEvents.errorEmit(validationExceptionToString(e));
            return Result.failed();
          }
          finally{      
            this._loaderService.hideIfVisible();
          }
          return Result.success();
    }
}