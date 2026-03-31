export class AnswerModel{
    
  answerId?: number;
  questionId?: number;
  appId?: number;
  answerOrder?: number;
  score?: number;
  text?: string;
  isCorrect: boolean = false;
  caseSensitive: boolean = false;
  response: number = 0;
}