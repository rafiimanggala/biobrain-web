import { distinct } from 'src/app/share/helpers/distinct-arrays';

export class GuidCollectionUpdateModel {
  public get all(): string[] {
    const toAdd = this.toAdd ?? [];
    const toRemove = this.toRemove ?? [];

    return distinct(toAdd.concat(toRemove));
  }

  constructor(
    public readonly toAdd?: string[],
    public readonly toRemove?: string[]
  ) {}

  static asDifferenceOf(newItems: string[], originalItems: string[]): GuidCollectionUpdateModel {
    return new GuidCollectionUpdateModel(
      newItems.filter(t => !originalItems.includes(t)),
      originalItems.filter(t => !newItems.includes(t))
    );
  }
}
