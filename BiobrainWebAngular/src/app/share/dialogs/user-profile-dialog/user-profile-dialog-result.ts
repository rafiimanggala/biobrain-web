export class UserProfileDialogResult {
  constructor(
    public readonly firstName: string,
    public readonly lastName: string,
    public readonly country: string,
    public readonly state: string,
    public readonly curriculumCode?: number,
  ) {
  }
}
