import { Injectable } from "@angular/core";
import { CurrentUserService } from "src/app/auth/services/current-user.service";
import { Dialog } from "src/app/core/dialogs/dialog.service";
import { Result, SuccessOrFailedResult } from "src/app/share/helpers/result";
import { SubscriptionListDialogData } from "../views/dialogs/subscription-list/subscription-list-dialog-data";
import { SubscriptionListDialog } from "../views/dialogs/subscription-list/subscription-list.dialog";

@Injectable({
    providedIn: 'root',
})
export class SubscriptionsListOperation {
    constructor(
        private readonly _dialog: Dialog,
        private readonly _currentUserService: CurrentUserService,
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
        
        
        await this._dialog.show(SubscriptionListDialog, new SubscriptionListDialogData(student.userId, student.firstName ?? student.name), {panelClass: "bordered-dialog-panel"});
        return Result.success();
    }
}