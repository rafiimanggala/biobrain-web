import { ContentTreeMeta } from 'src/app/api/content/content-data-models';

export class ContentTreeNodeMeta {
  constructor(
    public readonly dbRow: ContentTreeMeta,
    public readonly courseId: string,
  ) {
  }
}
