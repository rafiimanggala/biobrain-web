import { KeyValue } from "@angular/common";

export class AttachQuestionsToNodeCommand{
    constructor(
        public nodeId: string,
        public questionIds: Array<KeyValue<number, string>>,
    ){}
}