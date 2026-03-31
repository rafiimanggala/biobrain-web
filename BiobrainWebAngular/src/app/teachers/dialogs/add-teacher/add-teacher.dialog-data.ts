import { GetTeacherListItemsQuery_Result } from "src/app/api/teachers/get-teacher-list-items.query";

export class AddTeacherDialogData {
  constructor(
    public selectedTeacher: GetTeacherListItemsQuery_Result | null,
    public teachers: GetTeacherListItemsQuery_Result[]
  ) {}
}
