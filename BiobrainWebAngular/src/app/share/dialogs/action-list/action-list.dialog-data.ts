export class ActionListDialogData {
  constructor(
    public readonly title: string,
    public readonly text: string,
    public readonly actions: Action[]
  ) {}
}

export interface Action{
  text: string;
  code: number;
}
