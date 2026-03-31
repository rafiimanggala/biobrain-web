import { Moment } from "moment";

export class PickDateDialogData {
  constructor(
    public title: string,
    public date: Moment
  ) {
  }
}
