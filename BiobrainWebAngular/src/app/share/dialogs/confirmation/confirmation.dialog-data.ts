export class ConfirmationDialogData {
  constructor(
    public readonly title: string,
    public readonly text: string,
    public readonly yesButtonText?: string,
    public readonly noButtonText?: string,
  ) {}
}
