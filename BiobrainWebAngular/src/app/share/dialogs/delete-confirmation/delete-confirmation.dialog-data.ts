export class DeleteConfirmationDialogData {
  constructor(
    public entityType: string,
    public entityName: string,
    public confirmed?: boolean
  ) {}
}
