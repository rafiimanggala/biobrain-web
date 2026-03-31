import { Injectable } from '@angular/core';
import { Question } from 'src/app/api/content/content-data-models';
import { QuestionType } from 'src/app/api/enums/question-type.enum';
import { removeTags } from 'src/app/share/helpers/regex-helper';
import { StringsService } from 'src/app/share/strings.service';

@Injectable()
export class CorrectnessService {

  private readonly _responseRegex = /(response(\d)=([^&]+)&?)/g;
  private readonly _htmlTagRegEx = /<[^>]*>/g;

  constructor(protected readonly strings: StringsService) {
  }

  public checkIsCorrect(question: Question, answer: string): CorrectnessResult {
    const responses = this._parseAnswer(answer);
    const isCorrect = this._validateAnswer(question, responses);

    return {
      isCorrect,
      feedbackHtml: question.feedBack,
    };
  }

  private _validateAnswer(question: Question, responses: Response[]): boolean {
    switch (Number(question.questionTypeCode)) {
      case QuestionType.multipleChoice:
        return this._validateMultipleChoice(question, responses);
      case QuestionType.trueFalse:
        return this._validateTrueFalse(question, responses);
      case QuestionType.freeText:
        return this._validateFreeText(question, responses);
      case QuestionType.dropDown:
      case QuestionType.completeSentense:
        return this._validateDropDown(question, responses);
      case QuestionType.orderList:
        return this._validateOrderList(question, responses);
      case QuestionType.swipe:
        return this._validateSwipe(question, responses);
    }
    return false;
  }

  private _validateSwipe(question: Question, responses: Response[]): boolean {
    if (responses.length !== 2) {
      throw new Error(this.strings.incorrectAnswer);
    }
    console.log(responses);

    for (let i = 0; i < responses.length; i++) {
      const response = responses[i];
      let values = <string[]>JSON.parse(response.value);
      if (!values) {
        throw new Error(this.strings.incorrectAnswer);
      }

      values = values.map(x => removeTags(x).trim());
      const result = question.answers.filter(x => x.response === response.key).map(x => x.text.trim().replace(this._htmlTagRegEx, '')).every(x => values.some(y => x.localeCompare(y, undefined, { sensitivity: 'case' }) === 0));
      if (!result) {
        return false;
      }
    }
    return true;
  }

  private _validateOrderList(question: Question, responses: Response[]): boolean {
    console.log(responses);
    for (let i = 0; i < responses.length; i++) {
      const response = responses[i];
      const value = response.value.trim().replace(this._htmlTagRegEx, '');
      const answer = question.answers.find(x => x.text.replace(this._htmlTagRegEx, '').localeCompare(value, undefined, { sensitivity: 'case' }) === 0 && x.response === response.key);
      if (!answer) {
        return false;
      }
    }
    return true;
  }

  private _validateDropDown(question: Question, responses: Response[]): boolean {
    for (let i = 0; i < responses.length; i++) {
      const response = responses[i];
      var trimmedValue = response.value.trim();
      const value = trimmedValue.substring(1, trimmedValue.length - 1).replace(this._htmlTagRegEx, '');
      const answer = question.answers.find(x => x.text.replace(this._htmlTagRegEx, '').localeCompare(value, undefined, { sensitivity: 'case' }) === 0 && x.response === response.key);
      if (!answer || !answer.isCorrect) {
        return false;
      }
    }
    return true;
  }

  private _validateTrueFalse(question: Question, responses: Response[]): boolean {
    if (responses.length !== 1) {
      throw new Error(this.strings.incorrectAnswer);
    }
    const [response] = responses;
    const value = response.value.trim();
    const answer = question.answers.find(x => x.text.localeCompare(value, undefined, { sensitivity: 'accent' }) === 0);
    if (!answer || !answer.isCorrect) {
      return false;
    }
    return true;
  }

  private _validateFreeText(question: Question, responses: Response[]): boolean {
    if (responses.length !== 1) {
      throw new Error(this.strings.incorrectAnswer);
    }
    const [response] = responses;
    const value = response.value.substring(1, response.value.length - 1).trim().replace(this._htmlTagRegEx, '');
    const answer = question.answers.find(x => x.text.replace(this._htmlTagRegEx, '').localeCompare(value, undefined, { sensitivity: 'accent' }) === 0);

    if (!answer || !answer.isCorrect) {
      return false;
    }
    return true;
  }

  private _validateMultipleChoice(question: Question, responses: Response[]): boolean {
    if (responses.length !== 1) {
      throw new Error(this.strings.incorrectAnswer);
    }
    const [response] = responses;
    const answerNumber = Number(response.value);
    const answer = question.answers.find(x => x.answerOrder === answerNumber);
    if (!answer || !answer.isCorrect) {
      return false;
    }
    return true;
  }

  private _parseAnswer(answer: string): Response[] {
    const result: Response[] = [];
    let match;
    do {
      match = this._responseRegex.exec(answer);
      if (match) {
        result.push({
          key: Number(match[2]),
          value: match[3],
        });
      }
    } while (match);
    return result;
  }
}

export interface CorrectnessResult {
  isCorrect: boolean;
  feedbackHtml: string;
}

export interface Response {
  key: number;
  value: string;
}

