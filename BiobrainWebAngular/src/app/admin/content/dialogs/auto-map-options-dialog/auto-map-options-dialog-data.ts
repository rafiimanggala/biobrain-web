import { GetCourseQuizzesListQuery_Result } from "src/app/api/content/get-course-quizzes-list.query";

/* eslint-disable max-classes-per-file */
export class AutoMapOptionsDialogData {
  constructor(
    public quizId: string,
    public baseQuizzes: GetCourseQuizzesListQuery_Result[],
    public selectedBaseQuizId: string|undefined,
    public isStopAutoMapDisabled: boolean
  ) {}
}

export class AutoMapOptionsDialogResult {
  constructor(
    public readonly result: Result,
    public selectedBaseQuizId: string|undefined,
    public quizId: string,
  ) {
  }

  static stopAutoMapResult(data: AutoMapOptionsDialogData): AutoMapOptionsDialogResult {
    return new AutoMapOptionsDialogResult(
      Result.stop,
      data.selectedBaseQuizId,
      data.quizId
    );
  }

  static updateResult(data: AutoMapOptionsDialogData): AutoMapOptionsDialogResult {
    return new AutoMapOptionsDialogResult(
      Result.update,
      data.selectedBaseQuizId,
      data.quizId
    );
  }
}

export enum Result {
  stop,
  update,
}
