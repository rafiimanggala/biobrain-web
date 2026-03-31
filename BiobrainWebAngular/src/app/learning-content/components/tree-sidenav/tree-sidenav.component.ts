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
  }

  expandToLevel(maxLevel: number): void {
    const treeControl = this.treeService.flatTreeControl;
    treeControl.collapseAll();
    treeControl.dataNodes?.forEach((node: NodeModel) => {
      if (node.level < maxLevel) {
        treeControl.expand(node);
      }
    });
    this.allExpanded = false;
  }

  private _setSelectedTopic(topicId: string | undefined): void {
    // if (hasValue(topicId)) this.treeService.showNode(topicId);
    // else this._sidenavService.open();
    if (!hasValue(topicId))
      this._sidenavService.open();
  }
}
