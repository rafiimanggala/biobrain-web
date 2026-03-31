
import { NestedTreeNode } from "../../../core/services/nested-tree/neste-tree-node";

export class UserGuideNodeViewModel implements NestedTreeNode<UserGuideNodeViewModel>{
    public isSelected: boolean = false;
    constructor(
        public nodeId: string,
        public parentId: string,
        public name: string,
        public order: number,
        public isAvailableForStudent: boolean,
        public children: UserGuideNodeViewModel[]
    ){}
}