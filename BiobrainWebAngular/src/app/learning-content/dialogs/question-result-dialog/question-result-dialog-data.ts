import { Question } from '../../../api/content/content-data-models';

export class QuestionResultDialogData {
  constructor(
    public isCorrect: boolean,
    public isSecondTry: boolean,
    public question: Question,
  ) {
  }
}
