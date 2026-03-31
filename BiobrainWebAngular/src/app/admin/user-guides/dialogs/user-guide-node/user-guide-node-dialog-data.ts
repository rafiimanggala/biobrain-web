
export class UserGuideNodeDialogData {
  constructor(
    public name:string,
    public isAvailableForStudent: boolean,
    public isAvailabilityVisible: boolean = false,
    public isDeleteVisible: boolean = false
  ) { }
}
