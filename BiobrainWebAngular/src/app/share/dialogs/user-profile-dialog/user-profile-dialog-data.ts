export class UserProfileDialogData {
  constructor(
    public firstName: string,
    public lastName: string,
    public email: string,
    public userType: string,
    public hasCountry: boolean,
    public country: string = '',
    public state: string = '',
    public curriculumCode?: number,
  ) {
  }
}
