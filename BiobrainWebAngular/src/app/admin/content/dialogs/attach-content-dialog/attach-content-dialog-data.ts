import { GetContentTreeListQuery_Result } from '../../../../api/content/get-content-tree-list.query';

export class AttachContentDialogData {
  public isReplace: boolean = false;
  constructor(
    public entitiesToSelect: GetContentTreeListQuery_Result[],
    public idsToAttach: string[],
    public nodeId: string,
    public nodeSearchOpen: string
  ) {}
}
