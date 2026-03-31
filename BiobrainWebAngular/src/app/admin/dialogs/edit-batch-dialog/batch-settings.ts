import moment from "moment";
import { Moment } from "moment";

export class BatchSettings{
    public note: string = "";
    public courseIds: string[] = [];
    public numberOfCodes: number = 10;
    public expiryDate: Moment | undefined = moment().local().add(13, "months");
}