import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class SignUpStudentWithVoucherCommand extends Command<SignUpStudentWithVoucherCommand_Result> {
  constructor(
    public readonly email: string,
    public readonly password: string,
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly voucher: string,
    public readonly schoolName: string,
    public readonly country: string,
    public readonly state: string,
    public readonly curriculumCode?: number
  ) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.accounts()}/SignUpStudentWithVoucher`;
  }
}


export interface SignUpStudentWithVoucherCommand_Result {
  studentId: string;
}
