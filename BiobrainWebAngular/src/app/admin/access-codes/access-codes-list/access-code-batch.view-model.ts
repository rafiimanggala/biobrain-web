import { Moment } from "moment";
import { GetAccessCodesQuery_Result_AccessCode } from "src/app/api/access-codes/get-access-codes.query";

export interface AccessCodeBatchViewModel{
    batchId: string;
    batchHeader: string;
    codes: GetAccessCodesQuery_Result_AccessCode[];
    usedCodes: GetAccessCodesQuery_Result_AccessCode[];
    expiryDateUtc: string;
    expiryDateLocal: Moment;
    createdAtUtc: string;
}