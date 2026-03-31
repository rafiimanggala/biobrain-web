import { AnswerModel } from "./answer.model";

export class QuestionModel{    
  questionId?: number;
  topicId?: number;
  levelTypeId?: number;
  appId?: number;
  questionTypeId?: number;
  order?: number;
  header?: string;
  text?: string;
  hint?: string;
  feedback?: string;
  answers: AnswerModel[] = [];
  columns?: number;
}