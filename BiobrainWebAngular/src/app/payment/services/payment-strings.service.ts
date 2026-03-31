import { DatePipe } from "@angular/common";
import { Injectable } from "@angular/core";
import { PaymentPeriod } from "src/app/api/enums/payment-period.enum";
import { StringsService } from "src/app/share/strings.service";

@Injectable()
export class PaymentStringsService {

    get date(): string {
        return this._datepipe.transform(new Date(), 'dd/MM/yyyy') ?? "";
    }

    constructor(
        private readonly _strings: StringsService,
        private readonly _datepipe: DatePipe,
    ) { }

    getPeriodText(period: PaymentPeriod): string {
        switch (period) {
            case PaymentPeriod.Monthly: return this._strings.monthly;
            case PaymentPeriod.Yearly: return this._strings.annually;
        }
    }

    getNounPeriodText(period: PaymentPeriod): string {
        switch (period) {
            case PaymentPeriod.Monthly: return this._strings.month;
            case PaymentPeriod.Yearly: return this._strings.year;
        }
    }
}