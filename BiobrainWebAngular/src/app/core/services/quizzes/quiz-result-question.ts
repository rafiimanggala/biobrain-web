export class QuizResultQuestion {
  constructor(
    public readonly quizResultId: string,
    public readonly questionId: string,
    public readonly value: string,
    public readonly isCorrect: boolean,
  ) {
  }
}
