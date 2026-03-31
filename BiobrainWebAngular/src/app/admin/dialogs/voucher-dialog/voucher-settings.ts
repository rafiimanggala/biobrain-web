import moment from "moment";
import { Moment } from "moment";

export class VoucherSettings{
    public note:string = "";
    public totalAmount: number = 50;
    public country: string = "Australia";
    public expiryDate: Moment = moment().local().add(13, "months");
    public redeemExpiryDate: Moment = moment().local().add(13, "months");
    public numberOfCodes: number = 1;
}