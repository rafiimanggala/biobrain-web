import { HistoryQuizResult } from "./history-quiz-result";

export class QuizResultHistory {
    constructor(
        public averageQuizRate: number,
        public quizzesCompletedRate: number,
        public quizResults: HistoryQuizResult[],
    ) { }
}