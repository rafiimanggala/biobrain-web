import { ContentTreeMetaFilterValue } from "./content-tree-meta-filter-value.view-model";

export class ContentTreeMetaFilterViewModel{
    public selectedValue: ContentTreeMetaFilterValue|null;
    constructor(
        public contentTreeMetaId: string,
        public contentTreeMetaName: string,
        public depth: number,
        public values: ContentTreeMetaFilterValue[] = []
    ){
        this.selectedValue = values[0];
    }
}