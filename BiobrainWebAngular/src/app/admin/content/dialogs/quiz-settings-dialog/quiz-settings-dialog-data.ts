/* eslint-disable max-classes-per-file */
export class QuizSettingsDialogData {
  constructor(
    public quizId: string,
    public name: string,
    public questionCount: number | null,
    public totalQuestions: number,
    public randomizeOrder: boolean
  ) {}
}

export class QuizSettingsDialogResult {
  constructor(
    public readonly name: string,
    public readonly questionCount: number | null,
    public readonly randomizeOrder: boolean
  ) {}
}
