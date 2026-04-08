/* eslint-disable max-classes-per-file */
import { Question } from 'src/app/api/content/content-data-models';

export class QuizManagerDialogData {
  constructor(
    public readonly nodeId: string,
    public readonly courseId: string,
    public readonly quizId: string | null,
    public readonly quizName: string,
    public questions: Question[]
  ) {}
}

export class QuizManagerDialogResult {
  constructor(public readonly dirty: boolean) {}
}
