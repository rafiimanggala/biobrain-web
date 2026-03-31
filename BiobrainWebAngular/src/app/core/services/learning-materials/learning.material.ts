import { ContentTreeRow } from '../../../learning-content/services/learning-content-db';
import { ContentTreeNode } from '../content/content-tree.node';

export class LearningMaterial {
  constructor(
    public readonly row: ContentTreeRow,
    public readonly node: ContentTreeNode,
  ) {
  }

  public get fullName(): string {
    return this.node.fullName;
  }
}
