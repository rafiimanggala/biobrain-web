/* eslint-disable max-classes-per-file */
export class CreateQuestionDialogAnswer {
  constructor(
    public text: string,
    public isCorrect: boolean
  ) {}
}

export class CreateQuestionDialogData {
  constructor(
    public courseId: string,
    public nodeId: string | null,
    public questionId: string | null,
    public questionTypeCode: number,
    public header: string,
    public text: string,
    public hint: string,
    public feedBack: string,
    public answers: CreateQuestionDialogAnswer[]
  ) {}
}

export class CreateQuestionDialogResult {
  constructor(public readonly questionId: string) {}
}
