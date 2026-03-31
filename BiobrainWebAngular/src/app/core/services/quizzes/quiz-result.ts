import { QuizResultQuestion } from './quiz-result-question';

export class QuizResult {
  constructor(
    public readonly quizResultId: string,
    public readonly quizId: string,
    public readonly userId: string,
    public readonly schoolClassId: string,
    public readonly schoolId: string,
    public readonly schoolName: string,
    public readonly score: number,
    public readonly questions: QuizResultQuestion[],
    public readonly excludedQuestions: string[],
    public readonly hintsEnabled: boolean = true,
    public readonly soundEnabled: boolean = true
  ) {
  }
}
