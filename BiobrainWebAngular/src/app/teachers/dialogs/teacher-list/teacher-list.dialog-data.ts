import { GetTeacherListItemsQuery_Result } from "src/app/api/teachers/get-teacher-list-items.query";

export class TeacherListDialogData {
  constructor(
    public teachers: GetTeacherListItemsQuery_Result[],
    public classId: string
  ) {}
}
