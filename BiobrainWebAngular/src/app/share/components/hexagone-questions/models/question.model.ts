import { SafeHtml } from "@angular/platform-browser";

export class QuestionViewModel{
    constructor(
        public text: SafeHtml | null,
        public feedback: SafeHtml | null,
        public questionHeader: string,
        public correctAnswer: SafeHtml | null,
        public answers: SafeHtml[],
        public questionTypeCode: number,
        public questionId: string,
        public isExcluded: boolean
    ){}
}