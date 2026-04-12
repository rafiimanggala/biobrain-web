export class StarRatingDialogData {
  constructor(
    public readonly title: string = 'Rate BioBrain',
    public readonly userName: string = '',
  ) {}
}

export class StarRatingDialogResult {
  constructor(
    public readonly rating: number,
    public readonly feedback: string,
  ) {}
}
