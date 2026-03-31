export class SelectItemDialogData {
  constructor(
    public readonly entityName: string,
    public readonly items: SelectDialogItem[]
  ) { }
}

export interface SelectDialogItem {
  id: string;
  name: string;
}
