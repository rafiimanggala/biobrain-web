export class InformationDialogData {
  constructor(
    public readonly title: string,
    public readonly text: string,
    public readonly closeButtonText?: string,
    public readonly isHtmlText?: boolean
  ) {}
}
