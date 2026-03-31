export interface NestedTreeNode<T>{
    nodeId: string;
    parentId: string;
    order: number;
    children: T[];
}