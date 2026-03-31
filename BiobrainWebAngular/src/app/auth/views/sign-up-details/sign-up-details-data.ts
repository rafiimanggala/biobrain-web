export class SignUpDetailsData {
  constructor(
    public email: string = '',
    public firstName: string = '',
    public lastName: string = '',
    public password: string = '',
    public country: string = '',
    public state: string = '',
    public classCode?: string,
    public accessCode?: string,
    public voucher?: string,
    public schoolName: string = '',
    public curriculumCode?: number,
    public year?: number
  ) {
  }
}


export class SignUpDetailsSettings {
  public isClassCodeVisible: boolean = false;
  public isAccessCodeVisible: boolean = false;
  public isVoucherVisible: boolean = false;
}