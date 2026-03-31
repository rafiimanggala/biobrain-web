export class StarRatingDialogData {
  constructor(
    public readonly title: string = 'Rate BioBrain',
  ) {}
}

export class StarRatingDialogResult {
  constructor(
    public readonly rating: number,
    public readonly feedback: string,
  ) {}
}
