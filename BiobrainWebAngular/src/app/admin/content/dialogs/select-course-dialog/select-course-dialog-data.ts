import { GetCoursesListQuery_Result } from "src/app/api/content/get-courses-list.query";

/* eslint-disable max-classes-per-file */
export class SelectCourseDialogData {
  constructor(
    public courses: GetCoursesListQuery_Result[],
    public selectedCourseId: string | undefined
  ) {}
}

export class SelectCourseDialogResult{
  constructor(
    public courseId: string,
  ) {}
}
