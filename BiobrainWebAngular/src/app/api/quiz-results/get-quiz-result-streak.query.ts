import { ApiPath } from '../api-path.service';
import { Command } from '../common/command';

export class GetQuizResultStreakCommand extends Command<GetQuizResultStreakQuery_Result> {
  constructor(public readonly courseId: string, public readonly localDate: Date) {
    super();
  }

  getUrl(apiPath: ApiPath): string {
    return `${apiPath.quizResults()}/GetQuizResultStreak`;
  }
}

export interface GetQuizResultStreakQuery_Result {
  streak: number;
  daysCount: number;
}
