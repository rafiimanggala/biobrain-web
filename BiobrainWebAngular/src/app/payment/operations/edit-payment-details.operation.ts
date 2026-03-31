import { Injectable } from "@angular/core";
import { Api } from "src/app/api/api.service";
import { GetPaymentMethodsQuery } from "src/app/api/payments/get-pyment-methods.query";
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
import { PaymentDetailsDialog } from "../views/dialogs/payment-details/payment-details.dialog";
import { UpdatePaymentDetailsCommand } from "src/app/api/payments/update-payment-details.comnand";

@Injectable({
    providedIn: 'root',
})
export class EditPaymentDetailsOperation {
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
        const paymentMethods = await firstValueFrom(this._api.send(new GetPaymentMethodsQuery()));
        

        // Edit payment details
        const paymentDetailsDialogResult = await this._dialog.show(PaymentDetailsDialog, paymentMethods, {panelClass: "bordered-dialog-panel"});
        switch(paymentDetailsDialogResult.action){
            case DialogAction.save: {
               
                return await this.save(student.userId, paymentDetailsDialogResult.data?.cardToken);
            }
            case DialogAction.cancel: return Result.success();
            default: return Result.failed();
        }
    }

    private async save(userId: string, cardToken?: string){
        try {
            this._loaderService.show();
            await this._api.send(new UpdatePaymentDetailsCommand(userId, cardToken)).toPromise();
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

    // private async cancelSubscription(userId: string){
    //     try {
    //         this._loaderService.show();
    //         await this._api.send(new CancelScheduledPaymentCommand(userId)).toPromise();
    //         this._snackBarService.showMessage(this._strings.youHaveSuccessfullyRegistered);
    //         // await this._refreshTokenOperation.perform();
    //       }
    //       catch (e) {
    //         if (e instanceof BadRequestCommonException) this._appEvents.errorEmit(e.message);
    //         if (e instanceof InternalServerException) this._appEvents.errorEmit(e.message);
    //         if (e instanceof RequestValidationException) this._appEvents.errorEmit(validationExceptionToString(e));
    //         return Result.failed();
    //       }
    //       finally{      
    //         this._loaderService.hideIfVisible();
    //       }
    //       return Result.success();
    // }
}