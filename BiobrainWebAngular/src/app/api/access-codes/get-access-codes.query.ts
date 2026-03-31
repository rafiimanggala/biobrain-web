import { ApiPath } from '../api-path.service';
import { Query } from '../common/query';

export class GetAccessCodesQuery extends Query<GetAccessCodesQuery_Result> {
  constructor(
    public pageNumber: number,
    public pageSize: number
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accessCodes()}/GetAccessCodes`;
  }
}

export interface GetAccessCodesQuery_Result {
  batches: GetAccessCodesQuery_Result_Batch[];
  totalLength: number;
}

export interface GetAccessCodesQuery_Result_Batch{
  batchId: string;
  batchHeader: string;
  codes: GetAccessCodesQuery_Result_AccessCode[];
  usedCodes: GetAccessCodesQuery_Result_AccessCode[];
  expiryDateUtc: string;
  createdAtUtc: string;
}

export interface GetAccessCodesQuery_Result_AccessCode{
  accessCodeId: string;
  code: string;
  usedAtUtc: string | null;
}