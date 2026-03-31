import { QuizRow } from '../../../learning-content/services/learning-content-db';
import { ContentTreeNode } from '../content/content-tree.node';

export class Quiz {
  constructor(
    public readonly row: QuizRow,
    public readonly node: ContentTreeNode,
  ) {
  }

  public get fullName(): string {
    return this.node.fullName;
  }
}
