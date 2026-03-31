import { KeyValue } from "@angular/common";

export class AttachMaterialsToNodeCommand{
    constructor(
        public nodeId: string,
        public materialIds: Array<KeyValue<number, string>>,
    ){}
}