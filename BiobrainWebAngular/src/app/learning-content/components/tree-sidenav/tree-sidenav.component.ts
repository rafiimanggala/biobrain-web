import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

import { RoutingService } from '../../../auth/services/routing.service';
import { SidenavService } from '../../../core/services/side-nav.service';
import { hasValue } from '../../../share/helpers/has-value';
import { StringsService } from '../../../share/strings.service';
import { AssertNotAvailableInDemoOperation } from '../../operations/assert-not-available-in-demo.operation';
import { NodeModel } from '../../models/node.model';
import { TreeService } from '../../services/tree.service';

@Component({
  selector: 'app-tree-sidenav',
  templateUrl: './tree-sidenav.component.html',
  styleUrls: ['./tree-sidenav.component.scss'],
})
export class TreeSidenavComponent implements OnChanges {
  @Input() courseId?: string;
  @Input() topicId?: string;

  public searchString = '';
  public allExpanded = false;
  public l1Expanded = false;
  public l2Expanded = false;

  constructor(
    public readonly treeService: TreeService,
    public readonly strings: StringsService,
    private readonly _sidenavService: SidenavService,
    private readonly _routingService: RoutingService,
    private readonly _assertNotAvailableInDemoOperation: AssertNotAvailableInDemoOperation,
  ) {
  }

  async ngOnChanges(changes: SimpleChanges): Promise<void> {
    if (changes.courseId && hasValue(this.courseId)) await this.treeService.initTree(this.courseId);
    if (changes.topicId) this._setSelectedTopic(this.topicId);
  }

  onRouterLinkClick(): void {
    this._sidenavService.close();
  }

  async onUnavailableClick() {    
    await this._assertNotAvailableInDemoOperation.perform();
  }

  onSearchComplete(): void {
    if (!hasValue(this.courseId) || !hasValue(this.searchString) || this.searchString.length < 2) return;
    void this._routingService.navigateToMaterialsSearch(this.courseId, this.searchString);
  }

  toggleExpandAll(): void {
    if (this.allExpanded) {
      this.treeService.flatTreeControl.collapseAll();
    } else {
      this.treeService.flatTreeControl.expandAll();
    }
    this.allExpanded = !this.allExpanded;
    this.l1Expanded = false;
    this.l2Expanded = false;
  }

  toggleLevel(maxLevel: number): void {
    const isExpanded = maxLevel === 1 ? this.l1Expanded : this.l2Expanded;

    if (isExpanded) {
      this._collapseLevelScoped(maxLevel);
    } else {
      this._expandToLevelScoped(maxLevel);
    }

    if (maxLevel === 1) {
      this.l1Expanded = !this.l1Expanded;
      if (!this.l1Expanded) {
        this.l2Expanded = false;
      }
    } else if (maxLevel === 2) {
      this.l2Expanded = !this.l2Expanded;
      if (this.l2Expanded) {
        this.l1Expanded = true;
      }
    }
    this.allExpanded = false;
  }

  private _findContextParent(): NodeModel | null {
    const treeControl = this.treeService.flatTreeControl;
    const nodes = treeControl.dataNodes;
    if (!nodes) return null;

    const expandedParents = nodes.filter(
      (n: NodeModel) => n.expandable && treeControl.isExpanded(n)
    );
    if (expandedParents.length > 0) {
      return expandedParents[expandedParents.length - 1];
    }

    if (hasValue(this.topicId)) {
      const topicNode = nodes.find((n: NodeModel) => n.entityId === this.topicId);
      if (topicNode) {
        const idx = nodes.indexOf(topicNode);
        for (let i = idx - 1; i >= 0; i--) {
          if (nodes[i].level < topicNode.level && nodes[i].expandable) {
            return nodes[i];
          }
        }
      }
    }

    return null;
  }

  private _getDescendants(parent: NodeModel): NodeModel[] {
    const treeControl = this.treeService.flatTreeControl;
    const nodes = treeControl.dataNodes;
    if (!nodes) return [];

    const parentIdx = nodes.indexOf(parent);
    if (parentIdx < 0) return [];

    const descendants: NodeModel[] = [];
    for (let i = parentIdx + 1; i < nodes.length; i++) {
      if (nodes[i].level <= parent.level) break;
      descendants.push(nodes[i]);
    }
    return descendants;
  }

  private _expandToLevelScoped(maxLevel: number): void {
    const treeControl = this.treeService.flatTreeControl;
    const contextParent = this._findContextParent();

    if (contextParent) {
      const descendants = this._getDescendants(contextParent);
      treeControl.expand(contextParent);
      for (const node of descendants) {
        const relativeLevel = node.level - contextParent.level;
        if (relativeLevel <= maxLevel && node.expandable) {
          treeControl.expand(node);
        } else if (relativeLevel > maxLevel) {
          treeControl.collapse(node);
        }
      }
    } else {
      treeControl.dataNodes?.forEach((node: NodeModel) => {
        if (node.level < maxLevel) {
          treeControl.expand(node);
        }
      });
    }
  }

  private _collapseLevelScoped(maxLevel: number): void {
    const treeControl = this.treeService.flatTreeControl;
    const contextParent = this._findContextParent();

    if (contextParent) {
      const descendants = this._getDescendants(contextParent);
      for (const node of descendants) {
        const relativeLevel = node.level - contextParent.level;
        if (relativeLevel >= maxLevel) {
          treeControl.collapse(node);
        }
      }
    } else {
      treeControl.dataNodes?.forEach((node: NodeModel) => {
        if (node.level >= maxLevel - 1) {
          treeControl.collapse(node);
        }
      });
    }
  }

  private _setSelectedTopic(topicId: string | undefined): void {
    // if (hasValue(topicId)) this.treeService.showNode(topicId);
    // else this._sidenavService.open();
    if (!hasValue(topicId))
      this._sidenavService.open();
  }
}
