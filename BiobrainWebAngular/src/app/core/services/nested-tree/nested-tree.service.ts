import { NestedTreeControl } from "@angular/cdk/tree";
import { Injectable } from "@angular/core";
import { MatTreeNestedDataSource } from "@angular/material/tree";
import { NestedTreeNode } from "./neste-tree-node";

@Injectable()
export class NestedTreeService<T extends NestedTreeNode<T>>{

    public nestedTreeControl: NestedTreeControl<T>;
    public nestedDataSource: MatTreeNestedDataSource<T>;
    expandedNodes: T[] = [];

    constructor(){        
        this.nestedTreeControl = new NestedTreeControl<T>(this.getChildren.bind(this));
        this.nestedDataSource = new MatTreeNestedDataSource<T>();
    }

    getChildren(node: T): T[] {
      return node.children;
    }

    public getNode(id: string): T | null {
      for (let i = 0; i < this.nestedDataSource.data.length; i++) {
        const root = this.nestedDataSource.data[i];
        if (root.nodeId == id) {
          return root;
        }
  
        const descendants = this.nestedTreeControl.getDescendants(root);
        const node = descendants.find(n => n.nodeId === id);
        if (node) {
          return node;
        }
      }
      return null;
    }
  
    public getUpperSibling(node: T): T | null {
      const siblings = this.getSiblings(node);
      const uppers = siblings.filter(x => x.order < node.order).sort((a, b) => b.order - a.order);
      return uppers.length > 0 ? uppers[0] : null;
    }
  
    public getLowerSibling(node: T): T | null {
      const siblings = this.getSiblings(node);
      const uppers = siblings.filter(x => x.order > node.order).sort((a, b) => a.order - b.order);
      return uppers.length > 0 ? uppers[0] : null;
    }
  
    private getSiblings(node: T): T[] {
      let siblings: T[] = [];
      if (!node.parentId) {
        siblings = this.nestedDataSource.data;
      } else {
        const parent = this.getNode(node.parentId);
        // ToDo: redo
        if (!parent) {
          throw new Error("Can't find parent");
        }
        siblings = parent.children;
      }
      return siblings;
    }

    public initTree(tree: T[]): void {
      // this.nestedDataSource.data = null;
      this.saveExpandedNodes();
      this.nestedDataSource.data = [];
      this.nestedDataSource.data = tree;
      this.restoreExpandedNodes();
    }

    private saveExpandedNodes() {
      this.expandedNodes = [];
      this.getAllNodes().forEach(node => {
        if (this.nestedTreeControl.isExpanded(node)) {
          this.expandedNodes.push(node);
        }
      });
    }
  
    private getAllNodes(): T[] {
      const nodes = [];
      for (let i = 0; i < this.nestedDataSource.data.length; i++) {
        const root = this.nestedDataSource.data[i];
        nodes.push(root);
  
        const descendants = this.nestedTreeControl.getDescendants(root);
        nodes.push(...descendants);
      }
      return nodes;
    }
  
    private restoreExpandedNodes() {
      const nodes = this.getAllNodes();
      if(this.expandedNodes.length < 1 && this.nestedDataSource.data.length > 0) {
        this.nestedTreeControl.expand(this.nestedDataSource.data[0]);
      }

      this.expandedNodes.forEach(node => {
        const toExpand = nodes.find(n => n.nodeId === node.nodeId);
        if (toExpand) {
          this.nestedTreeControl.expand(toExpand);
        }
      });
    }

}