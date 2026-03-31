import { ContentTreeRow } from '../../../learning-content/services/learning-content-db';

export class ContentTreeNode {
  public readonly nodeId: string;
  public readonly namePath: string[];
  public readonly fullName: string;
  public readonly order: number;
  public parent: ContentTreeNode | undefined = undefined;
  public children: ContentTreeNode[] = [];
  public readonly isAvailableInDemo: boolean = false


  private readonly _root: ContentTreeNode | undefined;

  constructor(
    public readonly row: ContentTreeRow,
    public readonly parents: ContentTreeNode[],
  ) {
    this.parent = parents.length === 0 ? undefined : parents[parents.length - 1];
    this.namePath = [...parents.map(_ => _.row.name), this.row.name];
    this.fullName = [...parents, this].map(_ => _.row.name).join(' › ');
    this.order = row.order;
    this.nodeId = row.nodeId;
    this._root = parents.length === 0 ? undefined : parents.find(x => !x.parent);
    this.isAvailableInDemo = row.isAvailableInDemo;
  }

  get root(): ContentTreeNode {
    return this._root ? this._root : this;
  }
}
