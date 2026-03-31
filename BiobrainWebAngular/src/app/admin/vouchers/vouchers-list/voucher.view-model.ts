import { Moment } from "moment";

export interface VoucherViewModel{
    voucherId: string;
    code: string;
    note:string;
    totalAmount: number;
    amountUsed: number;
    country: string;
    expiryDateUtc: string;
    expiryDateLocal: Moment;
    redeemExpiryDateUtc: string;
    redeemExpiryDateLocal: Moment;
    createdAtUtc: string;
}