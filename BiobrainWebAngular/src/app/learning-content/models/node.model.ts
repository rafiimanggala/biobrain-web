export class NodeModel {
  constructor(
    public entityId: string,
    public header: string,
    public children: NodeModel[],
    public parentId: string,
    public order: number,
    public isAccent: boolean,
    public isBold: boolean,
    public routing: string,
    public isAvailableInDemo: boolean,
    public autoExpand: boolean = false,
    public isLoading: boolean = false,
    public expandable: boolean = false,
    public level: number = 0
  ) {}
}
